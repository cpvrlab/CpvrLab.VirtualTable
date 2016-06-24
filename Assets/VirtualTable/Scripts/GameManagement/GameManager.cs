using UnityEngine;
using System.Collections.Generic;

namespace CpvrLab.VirtualTable {

    public class GameManager : MonoBehaviour {

        public static GameManager instance { get { return _instance; } }
        private static GameManager _instance = null;

        public Game[] games;
        protected Game _currentGame;

        // list of players currently able to play
        protected List<GamePlayer> _players = new List<GamePlayer>();

        void Awake()
        {
            DontDestroyOnLoad(this);

            if(_instance != null) {
                Debug.LogError("GameManager: You are trying to instanciate multiple game managers, only one is allowed!");
                DestroyImmediate(this);
                return;
            }

            _instance = this;

            OnInitialize();
        }

        protected virtual void OnInitialize() { }

        public void AddPlayer(GamePlayer player)
        {
            if(!_players.Contains(player))
                _players.Add(player);
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
            if(index < 0 && games.Length <= index) {
                Debug.LogError("GameManager: Trying to load a game with an invalid index.");
                return;
            }

            StartGame(games[index]);
        }

        // todo:    allow for graceful starting and stopping of games
        //          use a callback paradigm to call start game as soon
        //          as the previous game has unloaded
        //          
        void StartGame(Game game)
        {
            StopGame(_currentGame);

            _currentGame = game;
            _currentGame.Initialize();

            _currentGame.GameFinished += GameFinished;
        }

        void StopGame(Game game)
        {
            if(game == null)
                return;

            _currentGame.Stop();
        }

        protected virtual void GameFinished(Game game)
        {
            _currentGame = null;
        }

        void FixedUpdate()
        {
            // advance current game logic
            if(_currentGame != null)
                _currentGame.OnUpdate();
        }

    }

}