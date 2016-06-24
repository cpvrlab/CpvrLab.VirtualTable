using UnityEngine;
using System.Collections.Generic;
using System;

namespace CpvrLab.VirtualTable {


    class ConnectFourPlayerData : GamePlayerData {

    }

    public class ConnectFourGame : Game {

        private ConnectFourPlayerData GetConcretePlayerData(int index)
        {
            return (ConnectFourPlayerData)_playerData[index];
        }

        protected override GamePlayerData CreatePlayerDataImpl(GamePlayer player)
        {
            return new ConnectFourPlayerData();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Debug.Log("ConnectFourGame: Initialized");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }

}