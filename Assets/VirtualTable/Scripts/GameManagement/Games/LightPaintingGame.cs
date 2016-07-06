using UnityEngine;
using System.Collections.Generic;

namespace CpvrLab.VirtualTable {

    class LightPaintingPlayerData : GamePlayerData {
        public LightPainter lightPainter;
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

                // unequip all of the items the player is using
                pd.player.UnequipAll();

                // todo: disable item pickups for the player

                // equip a light painter to the players main slot
                pd.lightPainter = Instantiate(lightPainterPrefab).GetComponent<LightPainter>();
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
                Destroy(pd.lightPainter.gameObject);                
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
    }

}