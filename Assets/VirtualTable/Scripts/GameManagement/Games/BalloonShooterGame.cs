using UnityEngine;
using System.Collections;
using System;

namespace CpvrLab.VirtualTable {

    class BalloonShooterPlayerData : GamePlayerData {

    }

    /// <summary>
    /// Balloon shooter game is played as follows: In the center of the play area a bunch of color coded
    /// balloons are released. Every payer gets an item that lets them pop thse balloons, for example a 
    /// gun or a bow. The players can't shoot immediatly however. The game will display a timer befor a single 
    /// balloon color is revelaed at which point the players have to shoot the matching balloon.
    /// 
    /// alternative idea:   Have a bunch of colored balloons released. Have a timer run down for maybe 30 seconds
    ///                     As the timer runs down more and more balloons will lose their color and fade to grey.
    ///                     Grey balloons reveal to the player that they aren't the target. As the timer nears zero
    ///                     so shrinks the possible options of balloons to shoot. Players have exactly three shots
    ///                     and a cooldown after each shot used. So the players have to decide if they want to waste shots early
    ///                     for a possible win or save them for when theres only about 5 balloons left.
    ///                     
    ///                     remaining time and shots used could both count toward an overall score for the player.
    /// </summary>
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
