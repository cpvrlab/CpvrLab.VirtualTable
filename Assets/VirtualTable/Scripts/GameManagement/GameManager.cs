﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace CpvrLab.VirtualTable {

    public class StartGameMessage : MessageBase
    {
        public int gameIndex = -1;
    }
    public class StopGameMessage : MessageBase
    { }


    /// <summary>
    /// The GameManager is responsible of loading and unloading games and everything related to it.
    /// If a game requires a scene change then this is also handled here. If a game is over the 
    /// game manager is also responsible of displaying the games results by rendering the games
    /// GamePlayerData in list form (this last part is still only an idea for the future)
    /// 
    /// note:   at the time of writing this comment the GameManager class is still pretty much untested
    ///         and may change immensely over the next few iterations.
    /// </summary>
    public class GameManager : NetworkBehaviour {

        public static GameManager instance { get { return _instance; } }
        private static GameManager _instance = null;

        public Game[] games;
        protected Game _currentGame;

        // list of players currently able to play
        protected List<GamePlayer> _players = new List<GamePlayer>();

        // dirty flag in case the players list has changed
        protected bool _dirty = false;

        void Awake()
        {
            DontDestroyOnLoad(this);

            if(_instance != null) {
                Debug.LogError("GameManager: You are trying to instanciate multiple game managers, only one is allowed!");
                DestroyImmediate(this);
                return;
            }

            _instance = this;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();


            OnInitialize();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.RegisterHandler(VTMsgType.StartGame, StartGameMsgHandler);
            NetworkServer.RegisterHandler(VTMsgType.StopGame, StopGameMsgHandler);
        }


        protected virtual void OnInitialize() { }

        public void AddPlayer(GamePlayer player)
        {
            if (!_players.Contains(player))
            {
                _players.Add(player);
                _dirty = true;
            }
            else
                Debug.LogError("GameManager: Trying to register the same player twice!");
        }

        public void AddPlayers(GamePlayer[] players)
        {
            foreach(var p in players) {
                AddPlayer(p);
            }
        }

        public void RemovePlayer(GamePlayer player)
        {
            _players.Remove(player);
        }

        public void RemovePlayers(GamePlayer[] players)
        {
            foreach(var p in players) {
                RemovePlayer(p);
            }
        }
        
        public void StartGame(int index)
        {
            if (isServer)
            {
                StartGameInternal(index);
            }
            else
            {
                var netMngr = NetworkManager.singleton as VTNetworkManager;
                var msg = new StartGameMessage();
                msg.gameIndex = index;
                netMngr.client.Send(VTMsgType.StartGame, msg);
            }
        }
        [Server] private void StartGameMsgHandler(NetworkMessage netMsg)
        {
            Debug.Log("GameManager: Received StartGameMsg");
            var msg = netMsg.ReadMessage<StartGameMessage>();
            StartGameInternal(msg.gameIndex);
        }


        [Server] public void StartGameInternal(int index)
        {
            if (index < 0 && games.Length <= index)
            {
                Debug.LogError("GameManager: Trying to load a game with an invalid index.");
                return;
            }

            StartGame(games[index]);
        }

        // todo:    allow for graceful starting and stopping of games
        //          use a callback paradigm to call start game as soon
        //          as the previous game has unloaded
        //          
        [Server] void StartGame(Game game)
        {
            // stop current game
            StopGame();

            _currentGame = game;
            _currentGame.Initialize();

            _currentGame.GameFinished += GameFinished;
        }

        [Server] void StopGame()
        {
            if(_currentGame == null)
                return;

            _currentGame.Stop();
        }

        protected virtual void GameFinished(Game game)
        {
            _currentGame = null;
        }

        void FixedUpdate()
        {
            if (!isServer)
                return;
            
            // todo: this dirty flag stuff is temporary... probably
            if(_dirty)
            {
                foreach (var game in games)                
                    game.AddPlayers(_players.ToArray());
                
                _dirty = false;
            }

            // advance current game logic
            if(_currentGame != null)
                _currentGame.OnUpdate();
        }
        

        [Server] private void StopGameMsgHandler(NetworkMessage netMsg)
        {
            Debug.Log("Received StopGameMsg");
            StopGame();
        }
    }

}