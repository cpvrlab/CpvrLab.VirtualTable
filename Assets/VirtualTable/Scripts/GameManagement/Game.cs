using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace CpvrLab.VirtualTable {

    /*
        Unclear stuff:
        1. how do we let the game manager know if the game has reached its player limit?
        2. how will we handle player limits in the future? Do people have to sign up in the lobby?



    */


    public class GamePlayerData {
        // todo: should this be public?
        public GamePlayer player;
    }


    public abstract class Game {

        protected bool _usingCustomScene = false;
        protected string _customSceneName = "";
        protected bool _isRunning = false;
        protected List<GamePlayerData> _playerData = new List<GamePlayerData>();

        protected int hubSceneIndex;

        // note: player limit is ignored for now
        protected bool _hasPlayerLimit;
        protected int _minPlayers;
        protected int _maxPlayers;

        protected float _gameTime;

        // Add a player to the game player list
        public void AddPlayer(GamePlayer player)
        {
            var element = _playerData.Find(e => e.player == player);

            if(element != null)
                _playerData.Add(CreatePlayerData(player));
            else
                Debug.LogWarning("Game WARNING: Careful, someone tried to add an already existing player to our list!");
        }

        // remove player from game player list
        public void RemovePlayer(GamePlayer player)
        {
            var element = _playerData.Find(e => e.player == player);

            if(element != null)
                _playerData.Remove(element);
            else
                Debug.LogWarning("Game WARNING: Careful, someone tried to remove a player that is not in the list!");
        }

        private GamePlayerData CreatePlayerData(GamePlayer player)
        {
            return CreatePlayerDataImpl(player);
        }

        protected abstract GamePlayerData CreatePlayerDataImpl(GamePlayer player);

        public void Initialize()
        {
            if(_usingCustomScene) {
                // todo: load scene gracefully (async) 
                SceneManager.LoadScene(_customSceneName);
            }

            OnInitialize();
        }

        // stops the current game gracefully
        public void Stop()
        {
            OnStop();
        }

        public void Update()
        {
            OnUpdate();
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnStop() { }
        protected virtual void OnUpdate() { }
    }

}