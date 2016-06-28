using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace CpvrLab.VirtualTable {

    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkPlayer : NetworkBehaviour {
        public override bool OnCheckObserver(NetworkConnection conn)
        {
            Debug.Log("NetworkPlayer: OnCheckObserver");
            return base.OnCheckObserver(conn);
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            Debug.Log("NetworkPlayer: OnDeserialize");
            base.OnDeserialize(reader, initialState);
        }

        public override void OnNetworkDestroy()
        {
            Debug.Log("NetworkPlayer: OnNetworkDestroy");
            base.OnNetworkDestroy();
        }

        public override bool OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
        {
            Debug.Log("NetworkPlayer: OnRebuildObservers");
            return base.OnRebuildObservers(observers, initialize);

        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            Debug.Log("NetworkPlayer: OnSerialize");
            return base.OnSerialize(writer, initialState);
        }

        public override void OnSetLocalVisibility(bool vis)
        {
            Debug.Log("NetworkPlayer: OnSetLocalVisibility");
            base.OnSetLocalVisibility(vis);
        }

        public override void OnStartAuthority()
        {
            Debug.Log("NetworkPlayer: OnStartAuthority");
            base.OnStartAuthority();
        }

        public override void OnStartClient()
        {
            Debug.Log("NetworkPlayer: OnStartClient");
            base.OnStartClient();
        }

        public override void OnStartLocalPlayer()
        {
            Debug.Log("NetworkPlayer: OnStartLocalPlayer");
            base.OnStartLocalPlayer();
        }

        public override void OnStartServer()
        {
            Debug.Log("NetworkPlayer: OnStartServer");
            base.OnStartServer();
        }

        public override void OnStopAuthority()
        {
            Debug.Log("NetworkPlayer: OnStopAuthority");
            base.OnStopAuthority();
        }

        public override void PreStartClient()
        {
            Debug.Log("NetworkPlayer: PreStartClient");
            base.PreStartClient();
        }
        
    }

}