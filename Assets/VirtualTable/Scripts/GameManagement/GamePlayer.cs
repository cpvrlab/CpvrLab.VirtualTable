using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

namespace CpvrLab.VirtualTable {

    // todo:    review the necessaty and function of this class
    //          it seems like the item equip handling could easily
    //          be done by concrete classes. Of course we need a base
    //          class to handle locking of input etc but maybe saving
    //          the input slots in here too is overkill and completely
    //          unnecessary. 
    //          Would it be better if we moved all the equip code into the
    //          UsableItem class?
    //
    public abstract class GamePlayer : NetworkBehaviour {
        

        // defines an input slot that can serve an usable item with input
        // todo: important, We need to define the attachment slots in this base class
        //          we need to do this because in case of a sudden disconnect we must
        //          properly unequip any items this player might have picked up before 
        //          destroying it. We probably have to move the handling of 
        //          assigning client authority to usableitems to the usableitem itself 
        //          for this to work.
        protected class AttachmentSlot
        {
            public GameObject attachPoint;
            public PlayerInput input;
            public UsableItem item = null;
        }
                
        // prevent the player from equipping or unequipping any items
        public bool lockEquippedItems = false;

        // prevent the player from picking up any items
        public bool lockPickup = false;

        // prevent the player from using any equipped items
        public bool lockItemUse = false;


        protected List<AttachmentSlot> _attachmentSlots = new List<AttachmentSlot>();

        public string displayName = "player";

        /// <summary>
        /// List of possible representations of this player
        /// This list is populated by the concrete class
        /// </summary>
        public List<PlayerModel> playerModels = new List<PlayerModel>();
        protected int _localPlayerModel = 0;
        protected int _remotePlayerModel = 1;
        protected PlayerModel _localPlayerModelInstance = null;
        protected PlayerModel _remotePlayerModelInstance = null;


        // todo:    use a unified naming scheme across all classes used for this project
        //          for the methods that can be used by subclasses. Maybe start isn't such a good idea
        //          we should maybe use an initialize method that gets called by start in the 
        public override void OnStartServer()
        {
            // add ourselves to the current game manager
            GameManager.instance.AddPlayer(this);
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
        }


        // todo: Can we rely on Start being called AFTER localplayerstart? Or more specific can we rely on 
        //          isLocalPlayer to contain the correct value every time? I'm not too sure this is the right
        //          approach and we might need our own implementation for this to work properly. For now
        //          this does the job and I'll just leave it be.
        public void Start()
        {
            // hacked this in quickly to test it out as soon as I can use the vive at work again
            // needs sanity checks, ability to switch local and remote models on the fly etc etc.
            if (isLocalPlayer && playerModels.Count > _localPlayerModel)
            {
                // todo: spawn local player model
                _localPlayerModelInstance = Instantiate(playerModels[_localPlayerModel], transform.position, transform.rotation) as PlayerModel;
                _localPlayerModelInstance.InitializeModel(this);
                _localPlayerModelInstance.playerText.text = displayName;
            }
            else if (playerModels.Count > _remotePlayerModel)
            {
                // todo: spawn remote player model
                _remotePlayerModelInstance = Instantiate(playerModels[_remotePlayerModel], transform.position, transform.rotation) as PlayerModel;
                _remotePlayerModelInstance.InitializeModel(this);
                _remotePlayerModelInstance.playerText.text = displayName;
            }
        }

        public override void OnNetworkDestroy()
        {
            Debug.Log("GamePlayer: OnNetworkDestroy");
            UnequipAll();
            base.OnNetworkDestroy();
        }

        protected void SetLocalPlayerModel(int index)
        {

        }
        
        protected AttachmentSlot GetAttachmentSlot(int index)
        {
            if(index < 0 || _attachmentSlots.Count <= index)
                return null;

            return _attachmentSlots[index];
        }

        protected AttachmentSlot FindAttachmentSlot(GameObject attachPoint)
        {
            return _attachmentSlots.Find(x => x.attachPoint == attachPoint);
        }

        protected AttachmentSlot FindAttachmentSlot(UsableItem item)
        {
            return _attachmentSlots.Find(x => x.item == item);
        }

        protected AttachmentSlot FindAttachmentSlot(PlayerInput input)
        {
            return _attachmentSlots.Find(x => x.input == input);
        }

        protected bool IsItemEquipped(UsableItem item)
        {
            return FindAttachmentSlot(item) != null;
        }

        protected bool IsAttachmentSlotInputAssigned(PlayerInput input)
        {
            return FindAttachmentSlot(input) != null;
        }

        protected int GetSlotIndex(AttachmentSlot slot)
        {
            return _attachmentSlots.IndexOf(slot);
        }

        /// <summary>
        /// Can be called on server and local player to equip an item
        /// to the main attachment slot.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="unequipIfOccupied"></param>
        public void Equip(UsableItem item,  bool unequipIfOccupied = false)
        {
            if (isLocalPlayer)
                Equip(GetMainInput(), item, unequipIfOccupied);
            else if(isServer)
            {
                // todo...
                //RpcEquipToMain(item.gameObject);
            }
        }
        


        // equip an item and assign it to a specific player input
        [Client]
        protected void Equip(PlayerInput input, UsableItem item, bool unequipIfOccupied = false)
        {
            if (item.isInUse)
            {
                Debug.LogError("GamePlayer: Trying to equip an item that is already in use!");
                return;
            }

            if (!isLocalPlayer)
            {
                Debug.LogError("GamePlayer: GamePlayer.Equip(input, item, unequipIfOccupied) can't be called from non local clients!");
                return;
            }

            var slot = FindAttachmentSlot(input);
            if(slot == null) {
                Debug.LogWarning("GamePlayer: Trying to add an item to a non existant attachment slot");
                return;
            }

            // already an item equipped to that slot
            if(slot.item != null && unequipIfOccupied) {
                // unequip the current item
                // todo: can we be sure that unequip will be executed on 
                //       all clients 
                Unequip(slot.item);                
            }

            // notify everyone about the equipped item
            CmdOnEquip(item.gameObject, GetSlotIndex(slot));
        }

        
        

        [Command] private void CmdOnEquip(GameObject item, int slotIndex) {
            // assign authority to the equipped item
            item.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
            
            RpcOnEquip(item, slotIndex);
        }
        [ClientRpc] public void RpcOnEquip(GameObject itemGameObject, int slotIndex)
        {
            var item = itemGameObject.GetComponent<UsableItem>();
            var slot = GetAttachmentSlot(slotIndex);
            
            if(item == null) {
                Debug.LogError("GamePlayer.RpcOnEquip(): Can't find attached UsableItem component!");
                return;
            }
            
            if(slot == null) {
                Debug.LogError("GamePlayer.RpcOnEquip(): Can't find attachment slot for index " + slotIndex + "!");
                return;
            }
            

            // attach the item
            // note: slot.input is null except for the local player
            //       and the item should also only have authority on the local player object
            //       since the comand call was made from there
            // todo: make sure that the input stuff is true
            slot.item = item;
            slot.item.AssignOwner(this, slot.input);
            slot.item.Attach(slot.attachPoint);
                        
            OnEquip(slot);
        }
        
        /// <summary>
        /// Can be called on client or server to unequip an item
        /// </summary>
        /// <param name="item"></param>
        public void Unequip(UsableItem item)
        {
            if (isLocalPlayer)
            {
                var slot = FindAttachmentSlot(item);
                if (slot == null)
                {
                    Debug.LogWarning("GamePlayer: Trying to unequip an item that wasn't equipped!");
                    return;
                }

                // notify everyone about the unequipped item
                CmdOnUnequip(item.gameObject);
            }
            else if(isServer)
            {
                // todo...
                // unsure if we need to separate server and client code here
            }
        }

        // unequip an item assigned to a specific slot
        [Client]
        protected void UnequipItemFrom(PlayerInput input)
        {
            var slot = FindAttachmentSlot(input);
            if(slot.item == null || !IsItemEquipped(slot.item))
                return;

            Unequip(slot.item);
        }

        /// <summary>
        /// Can be called on both server and local player
        /// to drop all equipped items
        /// </summary>
        public void UnequipAll()
        {
            foreach(var slot in _attachmentSlots) {
                if(slot.item != null) {
                    Unequip(slot.item);
                }
            }
        }

        [Command] private void CmdOnUnequip(GameObject item) {            
            RpcOnUnequip(item);
        }

        [ClientRpc] public void RpcOnUnequip(GameObject itemGameObject) {
            var item = itemGameObject.GetComponent<UsableItem>();

            if(item == null) {
                Debug.LogError("GamePlayer.RpcOnEquip(): Can't find attached UsableItem component!");
                return;
            }

            // call the unequip hook of the concrete GamePlayer
            OnUnequip(item);

            item.ClearOwner();
            item.ReleaseAuthority();
            item.Detach();         
        }

        // register a new input slot with the base class
        [Client]
        protected void AddAttachmentSlot(GameObject attachPoint, PlayerInput input = null)
        {
            if (FindAttachmentSlot(attachPoint) != null)
            {
                Debug.LogError("GamePlayer: Trying to add an attachment point that is already assigned to an attachment slot!");
                return;
            }

            if (input != null && IsAttachmentSlotInputAssigned(input))
            {
                Debug.LogError("GamePlayer: Trying to add a a player input that is already assign to an attachment slot!");
                return;
            }


            var attachmentSlot = new AttachmentSlot();
            attachmentSlot.attachPoint = attachPoint;
            attachmentSlot.input = input;

            _attachmentSlots.Add(attachmentSlot);
        }

        // todo: these functions don't need to be abstract anymore
        protected virtual void OnEquip(AttachmentSlot slot) { }
        protected virtual void OnUnequip(UsableItem item) { }
        protected abstract PlayerInput GetMainInput();
    }

}