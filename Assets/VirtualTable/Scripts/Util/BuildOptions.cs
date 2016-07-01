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
            Default,    // normal PC game build
            Vive,       // vive with motion controls
            Rift,        // rift with touch controls(?)
            Count
        }

        [Serializable]
        public struct AdvancedSettings {
            public NetworkManager networkManager;
        }

        [Serializable]
        public struct TargetOptions {
            public GameObject playerPrefab;
        }

        public Target buildTarget = Target.Default;
        public AdvancedSettings advancedSettings;
        public TargetOptions[] targetOptions;
        public bool logOutput = true;


        public void UpdateBuildTarget()
        {
            if(logOutput) { Debug.Log("BuildOptions changed, updating project accordingly."); }
            advancedSettings.networkManager.playerPrefab = targetOptions[(int)buildTarget].playerPrefab;
            if(logOutput) { Debug.Log("  Changing NetworkManager.playerPrefab to " + targetOptions[(int)buildTarget].playerPrefab); }
        }
    }
    

}