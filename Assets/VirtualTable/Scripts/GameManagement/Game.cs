﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

namespace CpvrLab.VirtualTable {

    /*
        Unclear stuff:
        1. how do we let the game manager know if the game has reached its player limit?
        2. how will we handle player limits in the future? Do people have to sign up in the lobby?



    */

    /// <summary>
    /// Base class for game player data. This class contains at least a reference to the GamePlayer
    /// that this data is describing. A concrete implementation of the class could keep track
    /// of player score or other relevant information a concrete game could need to store and display.
    /// 
    /// todo: Do we really need GamePlayer as a member of the data? Wouldn't a dictionary in the base
    /// Game class work much better for our usecases? Change if necessary.
    /// </summary>
    public class GamePlayerData {
        // todo: should this be public?
        public GamePlayer player;
    }

    /// <summary>
    /// Base class for all game implementations.
    /// 
    /// todo:   for now a Game is a MonoBehaviour so that we can edit it easier in the editor
    ///         but it would be great if we could edit the game rules etc in a custom list editor
    ///         in the game manager editor itself where each game exposes its settings
    ///         via a property drawer.
    /// </summary>
    public abstract class Game : MonoBehaviour {

        public event Action<Game> GameFinished;

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

        public void ClearPlayerList()
        {
            _playerData.Clear();
        }

        // Add a player to the game player list
        public void AddPlayer(GamePlayer player)
        {
            var element = _playerData.Find(e => e.player == player);

            if(element == null)
                _playerData.Add(CreatePlayerData(player));
            else
                Debug.LogWarning("Game WARNING: Careful, someone tried to add an already existing player to our list!");
        }

        public void AddPlayers(GamePlayer[] players)
        {
            foreach(var p in players) {
                AddPlayer(p);
            }
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

        public void RemovePlayers(GamePlayer[] players)
        {
            foreach(var p in players) {
                RemovePlayer(p);
            }
        }

        public void RemoveAllPlayers()
        {
            foreach(var p in _playerData)
                RemovePlayer(p.player);
        }

        private GamePlayerData CreatePlayerData(GamePlayer player)
        {
            var data = CreatePlayerDataImpl(player);
            data.player = player;

            return data;
        }

        protected abstract GamePlayerData CreatePlayerDataImpl(GamePlayer player);

        public void Initialize()
        {
            if(_usingCustomScene) {
                // todo: load scene gracefully (async) 
                SceneManager.LoadScene(_customSceneName);
            }

            // reset game time
            _gameTime = 0.0f;

            OnInitialize();
        }

        // stops the current game gracefully
        public void Stop()
        {
            OnStop();

            // notify others about the game ending
            if(GameFinished != null)
                GameFinished(this);
        }
        
        protected virtual void OnInitialize() { }
        protected virtual void OnStop() { }
        public virtual void OnUpdate() {
            _gameTime += Time.fixedDeltaTime;
        }
    }

}