using UnityEngine;
using UnityEngine.Networking;
using System;

namespace CpvrLab.VirtualTable {


    // this script handles the switching between
    // different build targets
    // it will adjust GUI setups and correct player prefab usage
    // etc...
    // todo: implement
    public class BuildOptions : MonoBehaviour {
        public enum Target {
            DefaultPC,    // normal PC game build
            Vive,       // vive with motion controls
            // Rift,        // rift with touch controls(?) not implemented yet
        }

        [Serializable]
        public struct AdvancedSettings {
            public NetworkManager networkManager;
        }

        [Serializable]
        public struct TargetOptions {
            public GameObject playerPrefab;
            public GameObject offlineCamera;
            public GameObject offlineMenu;
        }

        public Target buildTarget = Target.DefaultPC;
        public AdvancedSettings advancedSettings;
        public TargetOptions[] targetOptions;
        public bool logOutput = true;
        

        public void UpdateBuildTarget()
        {
            var selectedOption = targetOptions[(int)buildTarget];

            if (logOutput) { Log("BuildOptions changed, updating project accordingly."); }

            advancedSettings.networkManager.playerPrefab = selectedOption.playerPrefab;
            if(logOutput) { Log("Changing NetworkManager.playerPrefab to " + selectedOption.playerPrefab.name); }

            // disable all offline cameras and menus
            foreach(var option in targetOptions)
            {
                if (option.offlineCamera != null)
                    option.offlineCamera.SetActive(false);

                if (option.offlineMenu != null)
                    option.offlineMenu.SetActive(false);
            }
            // enable offline camera for selected target
            if (selectedOption.offlineCamera != null) selectedOption.offlineCamera.SetActive(true);
            if (logOutput) { Log("Changing offline camera to " + selectedOption.offlineCamera.name); }
            
            // enable offline menu for selected target
            if (selectedOption.offlineMenu != null) selectedOption.offlineMenu.SetActive(true);
            if (logOutput) { Log("Changing offline menu to " + selectedOption.offlineMenu.name); }
        }
        
        private void Log(string msg)
        {
            Debug.Log("BuildOptions: " + msg);
        }
    }
    

}