using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

namespace CpvrLab.VirtualTable {

    class LightPaintingPlayerData : GamePlayerData {
        public LightPainter lightPainter = null;
    }


    /// <summary>
    /// This isn't really a "game" but more of an expericne where every player 
    /// receives a light painting UsableItem to draw into the air. We could possibly add functionality like sharing drawings, saving
    /// or other collaboration stuff. Who knows, we'll see.
    /// </summary>
    public class LightPaintingGame : Game {

        public GameObject lightPainterPrefab;

        private LightPaintingPlayerData GetConcretePlayerData(int index)
        {
            return (LightPaintingPlayerData)_playerData[index];
        }

        protected override GamePlayerData CreatePlayerDataImpl(GamePlayer player)
        {
            return new LightPaintingPlayerData();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Debug.Log("LightPaintingGame: Initialized");

            // initialize our custom player data
            for(int i = 0; i < _playerData.Count; i++) {
                var pd = GetConcretePlayerData(i);
                Debug.Log("LightPaintingGame: " + i);

                // unequip all of the items the player is using
                pd.player.UnequipAll();

                // todo: disable item pickups for the player

                // equip a light painter to the players main slot
                if (pd.lightPainter == null)
                {
                    // if the light painter for this player hasn't been spawned do it now
                    pd.lightPainter = Instantiate(lightPainterPrefab).GetComponent<LightPainter>();
                    NetworkServer.Spawn(pd.lightPainter.gameObject);
                }
                pd.lightPainter.isVisible = true;
                pd.player.Equip(pd.lightPainter, true);
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            Debug.Log("LightPaintingGame: Stopping");
            

            for(int i = 0; i < _playerData.Count; i++) {
                var pd = GetConcretePlayerData(i);
                pd.player.UnequipAll();

                // hide objects
                pd.lightPainter.isVisible = false;
                
                //NetworkServer.Destroy(pd.lightPainter.gameObject);
                //Destroy(pd.lightPainter.gameObject);
                //pd.lightPainter = null;           
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            // automatically stop the game after 60 seconds
            // this is just a test
            Debug.Log(_gameTime);

            if(_gameTime > 10.0f)
                Stop();
        }

        protected override string GetGameName()
        {
            throw new NotImplementedException();
        }

        protected override string[] GetScoreTitles()
        {
            throw new NotImplementedException();
        }

        protected override string[] GetScoreValues(int playerIndex)
        {
            throw new NotImplementedException();
        }

        protected override int GetScoreColumnCount()
        {
            throw new NotImplementedException();
        }
    }

}