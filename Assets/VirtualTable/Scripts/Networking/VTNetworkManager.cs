using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Networking.Match;

namespace CpvrLab.VirtualTable {

    // test class to see how unity's networkmanager works
    public class VTNetworkManager : NetworkManager {

        public override NetworkClient StartHost()
        {
            Debug.Log("NetworkManager: StartHost");
            return base.StartHost();
        }

        public override void ServerChangeScene(string newSceneName)
        {
            Debug.Log("NetworkManager: ServerChangeScene");
            base.ServerChangeScene(newSceneName);
        }

        public override NetworkClient StartHost(ConnectionConfig config, int maxConnections)
        {
            Debug.Log("NetworkManager: StartHost");
            return base.StartHost(config, maxConnections);
        }

        public override NetworkClient StartHost(MatchInfo info)
        {
            Debug.Log("NetworkManager: StartHost");
            return base.StartHost(info);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            Debug.Log("NetworkManager: OnClientConnect");
            base.OnClientConnect(conn);
        }

        public override void OnStartServer()
        {
            Debug.Log("NetworkManager: OnStartServer");
            base.OnStartServer();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            Debug.Log("NetworkManager: OnClientDisconnect");
            base.OnClientDisconnect(conn);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            Debug.Log("NetworkManager: OnClientError");
            base.OnClientError(conn, errorCode);
        }
        public override void OnClientNotReady(NetworkConnection conn)
        {
            Debug.Log("NetworkManager: OnClientNotReady");
            base.OnClientNotReady(conn);
        }
        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            Debug.Log("NetworkManager: OnClientSceneChanged");
            base.OnClientSceneChanged(conn);
        }
        public override void OnMatchCreate(CreateMatchResponse matchInfo)
        {
            Debug.Log("NetworkManager: OnMatchCreate");
            base.OnMatchCreate(matchInfo);
        }
        public override void OnMatchList(ListMatchResponse matchList)
        {
            Debug.Log("NetworkManager: OnMatchList");
            base.OnMatchList(matchList);
        }
        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            Debug.Log("NetworkManager: OnServerAddPlayer");
            base.OnServerAddPlayer(conn, playerControllerId);
        }
        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
        {
            Debug.Log("NetworkManager: OnServerAddPlayer");
            base.OnServerAddPlayer(conn, playerControllerId, extraMessageReader);
        }
        public override void OnServerConnect(NetworkConnection conn)
        {
            Debug.Log("NetworkManager: OnServerConnect");
            base.OnServerConnect(conn);
        }
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            Debug.Log("NetworkManager: OnServerDisconnect");
            base.OnServerDisconnect(conn);
        }
        public override void OnServerError(NetworkConnection conn, int errorCode)
        {
            Debug.Log("NetworkManager: OnServerError");
            base.OnServerError(conn, errorCode);
        }
        public override void OnServerReady(NetworkConnection conn)
        {
            Debug.Log("NetworkManager: OnServerReady");
            base.OnServerReady(conn);
        }
        public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
        {
            Debug.Log("NetworkManager: OnServerRemovePlayer");
            base.OnServerRemovePlayer(conn, player);
        }
        public override void OnServerSceneChanged(string sceneName)
        {
            Debug.Log("NetworkManager: OnServerSceneChanged");
            base.OnServerSceneChanged(sceneName);
        }
        public override void OnStartClient(NetworkClient client)
        {
            Debug.Log("NetworkManager: OnStartClient");
            base.OnStartClient(client);
        }
        public override void OnStartHost()
        {
            Debug.Log("NetworkManager: OnStartHost");
            base.OnStartHost();
        }
        public override void OnStopClient()
        {
            Debug.Log("NetworkManager: OnStopClient");
            base.OnStopClient();
        }
        public override void OnStopHost()
        {
            Debug.Log("NetworkManager: OnStopHost");
            base.OnStopHost();
        }
        public override void OnStopServer()
        {
            Debug.Log("NetworkManager: OnStopServer");
            base.OnStopServer();
        }
    }

}
