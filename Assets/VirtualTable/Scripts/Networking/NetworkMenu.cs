using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


namespace CpvrLab.VirtualTable {

    /// <summary>
    /// Handles everything related to the start screen menu where players connect to 
    /// or create a server to play on.
    /// </summary>
    public class NetworkMenu : MonoBehaviour {

        public InputField playerNameInput;
        public InputField ipInput;
        public InputField portInput;

        public void OnConnectClicked()
        {
            Debug.Log("Connectclicked");
            var netMngr = NetworkManager.singleton;

            netMngr.networkAddress = ipInput.text;
            netMngr.networkPort = int.Parse(portInput.text);

            Debug.Log("Trying to connect to " + netMngr.networkAddress + ":" + netMngr.networkPort);
            
            netMngr.StartClient();
        }

        public void OnHostClicked()
        {
            var netMngr = NetworkManager.singleton;
            netMngr.networkPort = int.Parse(portInput.text);

            netMngr.StartHost();
        }

        public void OnNameInputChanged(string text)
        {
            var netMngr = (VTNetworkManager)NetworkManager.singleton;
            netMngr.localPlayerName = text;
        }
    }

}