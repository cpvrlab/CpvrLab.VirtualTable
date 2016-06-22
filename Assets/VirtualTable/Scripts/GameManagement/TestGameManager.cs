using UnityEngine;
using System.Collections;


namespace CpvrLab.VirtualTable {

    public class TestGameManager : GameManager {
        protected override void OnInitialize()
        {
            base.OnInitialize();

            // note: we currently add our games manually
            // todo: create editor for managing games in the game managers list including their settings
            _games.Add(new LightPaintingGame());
            _games.Add(new ConnectFourGame());
        }
    }

}