using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


namespace CpvrLab.VirtualTable {

    public class NetworkMenu : MonoBehaviour {

        public InputField playerNameInput;
        public InputField ipInput;
        public InputField portInput;

        public void OnConnectClicked()
        {
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
    }

}