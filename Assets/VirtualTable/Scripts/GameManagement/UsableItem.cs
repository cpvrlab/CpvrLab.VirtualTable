using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace CpvrLab.VirtualTable {

    /// <summary>
    /// Base class for all usable items. A usable item is an object that can be equipped by a GamePlayer.
    /// When equipped by a GamePlayer a UsableItem will be attachd to an attachment point defined by the
    /// GamePlayer and receive input from one of the GamePlayer's PlayerInput components.
    /// 
    /// todo:   properly sync the equip state of this item. If a player connects late he must be able
    ///         to know which items are in use and which ones he can safely pick up.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(NetworkTransform))]
    public class UsableItem : NetworkBehaviour {

        public Transform attachPoint;
        protected PlayerInput _input;
        protected Transform _prevParent = null;
        protected GamePlayer _owner = null;
        public bool isInUse { get { return _owner != null; } }
        [SyncVar] protected bool _unequipDone;
        [SyncVar(hook ="OnVisibilityChanged")] public bool isVisible = true;
        [SyncVar] public bool inputEnabled = true;

        private void OnVisibilityChanged(bool value)
        {
            isVisible = value;
            for(int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(value);
            
            GetComponent<Rigidbody>().isKinematic = !value;
        }

        // tempararily used for debugging purposes
        public override void OnStartAuthority()
        {
            Debug.Log("Start authority");
            base.OnStartAuthority();
        }

        // tempararily used for debugging purposes
        public override void OnStopAuthority()
        {
            Debug.Log("Stop authority");
            base.OnStopAuthority();
        }

        /// <summary>
        /// Attaches this usable item to a given attachment point.
        /// Currently this is done by setting the rigidbody of the item to
        /// be kinematic and childing it to the attach GameObject.
        /// 
        /// Concrete GamePlayers can change the local position and rotation of the item
        /// by overriding GamePlayer.OnEquip and changing the values there.
        /// </summary>
        /// <param name="attach"></param>
        [Client] public void Attach(GameObject attach)
        {
            _prevParent = transform.parent;

            transform.parent = attach.transform;
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
            
            // "disable" rigidbody by setting it to kinematic
            var rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        /// <summary>
        /// Detaches an item from the current attachment point.
        /// </summary>
        [Client] public void Detach()
        {
            // return if we're not attached to anything.
            if (transform.parent == _prevParent)
                return;

            // todo: remove debug output
            Debug.Log("Detach " + ((_prevParent != null) ? _prevParent.name : "null"));

            transform.parent = _prevParent;
            var rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
        }
        
        /// <summary>
        /// Assign an owner of this UsableItem. If the local GamePlayer is the owner then input will
        /// contain a non null value. Else only owner will be assigned.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="input"></param>
        [Client] public void AssignOwner(GamePlayer owner, PlayerInput input) {
            _owner = owner;
            _input = input;

            OnEquip();
        }

        /// <summary>
        /// OnEquip is the initialization method for concrete UsableItems and is called on all client representations.
        /// 
        /// todo: find a better name for this.
        /// </summary>
        [Client] protected virtual void OnEquip()
        {
            Debug.Log("OnEquip");
        }

        [Client] public void ClearOwner() {
            _owner = null;
            _input = null;

            OnUnequip();
        }

        /// <summary>
        /// OnUnequip is the last method called before this item loses its owner. Used for item cleanup if necessary.
        /// </summary>
        [Client] protected virtual void OnUnequip()
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

    }
}