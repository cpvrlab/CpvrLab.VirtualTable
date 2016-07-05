using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace CpvrLab.VirtualTable {

    [RequireComponent(typeof(Rigidbody), typeof(NetworkTransform))]
    public class UsableItem : NetworkBehaviour {

        public Transform attachPoint;
        protected PlayerInput _input;
        protected Transform _prevParent = null;
        protected GamePlayer _owner = null;
        public bool isInUse { get { return _owner != null; } }
        [SyncVar] protected bool _unequipDone;
        
        public override void OnStartAuthority()
        {
            Debug.Log("Start authority");
            base.OnStartAuthority();
        }

        public override void OnStopAuthority()
        {
            Debug.Log("Stop authority");
            base.OnStopAuthority();
        }

        [Client]
        public void Attach(GameObject attach)
        {
            _prevParent = transform.parent;

            transform.parent = attach.transform;
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;

            // todo: not sure if it's a good idea to handle this here.
            // "disable" rigidbody by setting it to kinematic
            var rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        [Client]
        public void Detach()
        {
            Debug.Log("Detach " + ((_prevParent != null) ? _prevParent.name : "null"));
            transform.parent = _prevParent;
            var rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
        }

        [Client]
        public void AssignOwner(GamePlayer owner, PlayerInput input) {
            _owner = owner;
            _input = input;

            OnEquip();
        }

        [Client]
        protected virtual void OnEquip()
        {
            Debug.Log("OnEquip");
        }

        [Client]
        public void ClearOwner() {
            _owner = null;
            _input = null;

            OnUnequip();
        }

        [Client]
        protected virtual void OnUnequip()
        {
            Debug.Log("OnUnequip");
        }



        // Release client authority
        // todo:    implement a more reliable solution for the problem of releasing authority
        //          after OnUnequip has been called.
        //          the problem: OnUnequip is called, a concrete UsableObject might want to
        //          send off some final commands in there and have them executed on all clients
        //          if we'd release authority in OnUnequip the RPC calls wouldn't trigger anymore
        //          resulting in mismatches for different clients
        //          The below implementation won't fix this in all instances
        //          all it does is do an additional cycle of client -> server -> client
        //          before actually releasing authority. But it works for now and I have more
        //          immediate concerns right now than implementing this properly.
        //          You're welcome 'future me'
        public void ReleaseAuthority() { if(hasAuthority) CmdReleaseAuthorityDelay(); }
        [Command] private void CmdReleaseAuthorityDelay() { RpcReleaseAuthorityDelay(); }
        [ClientRpc] private void RpcReleaseAuthorityDelay() { if(hasAuthority) CmdReleaseAuthority(); }        
        [Command] private void CmdReleaseAuthority()
        {
            Debug.Log("Releasing client authority");
            var nId = GetComponent<NetworkIdentity>();
            nId.RemoveClientAuthority(nId.clientAuthorityOwner);
        }
        

        // 
    }
}