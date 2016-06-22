using UnityEngine;
using System.Collections;
using System;

namespace CpvrLab.VirtualTable {

    class LightPaintingPlayerData : GamePlayerData {

    }

    public class LightPaintingGame : Game {

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
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }
    }

}