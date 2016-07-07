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
            public VTNetworkManager networkManager;
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

            // find out which index our prefab has in the network manager
            var prefabs = advancedSettings.networkManager.playerPrefabs;
            int prefabIndex = -1;
            for (int i = 0; i < prefabs.Length; ++i) {
                if (prefabs[i].Equals(targetOptions[(int)buildTarget].playerPrefab))
                {
                    prefabIndex = i;
                    break;
                }
            }

            if(prefabIndex == -1)
            {
                LogError("Couldn't find the player prefab in the network manager. Make sure to add a player prefab for your custom player to both the network manager and to the build options!");
            }
            else
            {
                advancedSettings.networkManager.networkPrefabIndex = prefabIndex;
            }


            // disable all offline cameras and menus
            foreach (var option in targetOptions)
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
        private void LogError(string msg)
        {
            Debug.LogError("BuildOptions: " + msg);
        }
    }
    

}