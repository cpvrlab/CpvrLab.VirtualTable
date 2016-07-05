using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Networking.Match;

namespace CpvrLab.VirtualTable
{


    public class VTMsgType
    {
        public static short AddPlayer = MsgType.Highest + 1;
    };
    public class AddPlayerMessage : MessageBase
    {
        public string name;
        public int playerPrefabIndex = 0;
    }

    // test class to see how unity's networkmanager works
    public class VTNetworkManager : NetworkManager
    {

        //public NetworkPlayer playerPrefab;


        public int networkPrefabIndex = 0;
        public GameObject[] playerPrefabs;

        // client variable containing the player name of the local player as
        // set in the connect screen
        [HideInInspector]
        public string localPlayerName = "player";


        protected NetworkPlayer _playerInstance;

        void Start()
        {
            //LogFilter.currentLogLevel = (int)LogFilter.FilterLevel.Debug;
        }

        /// <summary>
        /// Currently an ugly workaround for players who suddenly disconnect and still have usable
        /// items with client authority set. We remove the client authority on every item for this player
        /// before they get destroyed.
        /// todo: find a better solution. 
        /// </summary>
        /// <param name="conn"></param>
        public override void OnServerDisconnect(NetworkConnection conn)
        {

            var usableItems = FindObjectsOfType<UsableItem>();
            foreach(var item in usableItems)
            {
                Debug.Log("Removing authority from " + item.name + " " + item.GetComponent<NetworkIdentity>().clientAuthorityOwner + "  " + conn);
                var networkId = item.GetComponent<NetworkIdentity>();
                if (networkId.clientAuthorityOwner != null && conn == networkId.clientAuthorityOwner)
                {
                    networkId.RemoveClientAuthority(networkId.clientAuthorityOwner);
                }
            }

            base.OnServerDisconnect(conn);
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            base.OnServerAddPlayer(conn, playerControllerId);
            //playerPrefab
            Debug.Log("OnServerAddPlayer");
        }

        public override void OnStartServer()
        {
            base.OnStartServer();


        }

        public override void OnStartClient(NetworkClient client)
        {
            base.OnStartClient(client);
            foreach (var prefab in playerPrefabs)
                ClientScene.RegisterPrefab(prefab);
        }
        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader netMsg)
        {
            //base.OnServerAddPlayer(conn, playerControllerId, netMsg);


            var msg = netMsg.ReadMessage<AddPlayerMessage>();
            Debug.Log("Adding player... " + msg.name + " " + playerPrefabs[msg.playerPrefabIndex].name);
            GameObject player = (GameObject)Instantiate(playerPrefabs[msg.playerPrefabIndex], Vector3.zero, Quaternion.identity);

            var gamePlayer = player.GetComponent<GamePlayer>();
            gamePlayer.displayName = msg.name;

            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }
        

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);
            Debug.Log("OnClientSceneChanged");

            // spawn our player
            var msg = new AddPlayerMessage();
            msg.playerPrefabIndex = networkPrefabIndex;
            msg.name = localPlayerName;
            ClientScene.AddPlayer(conn, 0, msg);
        }


        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);
            Debug.Log("OnServerSceneChanged");
        }

        /*
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
        }*/
    }

}
