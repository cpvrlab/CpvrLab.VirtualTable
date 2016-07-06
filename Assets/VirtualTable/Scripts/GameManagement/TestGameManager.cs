using UnityEngine;
using System.Collections;


namespace CpvrLab.VirtualTable {

    /// <summary>
    /// This game manager was used for early tests. Will probably be removed pretty soon.
    /// </summary>
    public class TestGameManager : GameManager {
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            // temp hack, we don't have access to the real player data
            // at the time we start here 
            // because at the moment we add the layers in to the game manager
            // in the Start method of the GamePlayer class
            AddPlayers(FindObjectsOfType<GamePlayer>());

            // for testing purposes we add all of the players to all of our games. 
            // todo: add a functionality for players to opt out of or opt in to 
            //       participating in a game, for each game.
            foreach(var game in games) {
                game.AddPlayers(_players.ToArray());
            }
            
        }

        protected override void GameFinished(Game game)
        {
            base.GameFinished(game);
            Debug.Log("TestGameManager: Game finished " + game.name);
        }
    }

}