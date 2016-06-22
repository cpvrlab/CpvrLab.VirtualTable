using UnityEngine;
using System.Collections.Generic;

namespace CpvrLab.VirtualTable {

    public class GameManager : MonoBehaviour {

        public static GameManager instance { get { return _instance; } }
        private static GameManager _instance = null;

        protected List<Game> _games = new List<Game>();
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

        public void RemovePlayer(GamePlayer player)
        {
            _players.Remove(player);
        }

        public void StartGame(int index)
        {
            if(index < 0 && _games.Count <= index) {
                Debug.LogError("GameManager: Trying to load a game with an invalid index.");
                return;
            }

            StartGame(_games[index]);
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
        }

        void StopGame(Game game)
        {
            if(game == null)
                return;

            _currentGame.Stop();
        }

        void Update()
        {
            // advance current game logic
            if(_currentGame != null)
                _currentGame.Update();
        }

    }

}