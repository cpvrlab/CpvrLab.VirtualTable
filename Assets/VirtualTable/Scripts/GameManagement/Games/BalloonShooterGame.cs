using UnityEngine;
using System.Collections;
using System;

namespace CpvrLab.VirtualTable {

    class BalloonShooterPlayerData : GamePlayerData {

    }

    public class BalloonShooterGame : Game {

        private BalloonShooterPlayerData GetConcretePlayerData(int index)
        {
            return (BalloonShooterPlayerData)_playerData[index];
        }

        protected override GamePlayerData CreatePlayerDataImpl(GamePlayer player)
        {
            return new BalloonShooterPlayerData();
        }
    }

}
