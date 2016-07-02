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
        protected class InputSlot {
            public PlayerInput input;
            public GameObject attachPoint;
            public UsableItem item = null;
        }
                
        // prevent the player from equipping or unequipping any items
        public bool lockEquippedItems = false;

        // prevent the player from picking up any items
        public bool lockPickup = false;

        // prevent the player from using any equipped items
        public bool lockItemUse = false;


        protected List<InputSlot> _inputSlots = new List<InputSlot>();

        /// <summary>
        /// List of possible representations of this player
        /// This list is populated by the concrete class
        /// </summary>
        public List<PlayerModel> playerModels = new List<PlayerModel>();
        protected int _localPlayerModel = 0;
        protected int _remotePlayerModel = 0;
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

        public override void OnStartClient()
        {
            base.OnStartClient();

            // hacked this in quickly to test it out as soon as I can use the vive at work again
            // needs sanity checks, ability to switch local and remote models on the fly etc etc.
            if(isLocalPlayer && playerModels.Count > _localPlayerModel)
            {
                // todo: spawn local player model
                _localPlayerModelInstance = Instantiate(playerModels[_localPlayerModel], transform.position, transform.rotation) as PlayerModel;
                _localPlayerModelInstance.InitializeModel(this);
            }
            else if(playerModels.Count > _remotePlayerModel)
            {
                // todo: spawn remote player model
                _remotePlayerModelInstance = Instantiate(playerModels[_remotePlayerModel], transform.position, transform.rotation) as PlayerModel;
                _remotePlayerModelInstance.InitializeModel(this);
            }
        }

        public override void OnNetworkDestroy()
        {
            Debug.Log("GamePlayer: OnNetworkDestroy");
            UnequipAllImmediate();
            base.OnNetworkDestroy();
        }

        protected void SetLocalPlayerModel(int index)
        {

        }
        
        protected InputSlot GetInputSlot(int index)
        {
            if(index < 0 || _inputSlots.Count <= index)
                return null;

            return _inputSlots[index];
        }

        protected InputSlot FindInputSlot(UsableItem item)
        {
            return _inputSlots.Find(x => x.item == item);
        }

        protected InputSlot FindInputSlot(PlayerInput input)
        {
            return _inputSlots.Find(x => x.input == input);
        }

        protected bool IsItemEquipped(UsableItem item)
        {
            return FindInputSlot(item) != null;
        }

        protected bool IsInputSlotAssigned(PlayerInput input)
        {
            return FindInputSlot(input) != null;
        }

        // equip an item and assign it to a specific player input
        public void Equip(PlayerInput input, UsableItem item, bool unequipIfOccupied = false)
        {
            var slot = FindInputSlot(input);
            if(slot == null) {
                Debug.LogWarning("GamePlayer: Trying to add an item to a non existant input slot");
                return;
            }

            // already an item equipped to that slot
            if(slot.item != null) {
                if(!unequipIfOccupied)
                    return;

                // unequip the current item
                Unequip(slot.item);
            }

            // assign new the item to the slot
            slot.item = item;

            // notify everyone about the equipped item
            CmdOnEquip(item.gameObject);
        }

        // equip an item to the main input slot
        public void Equip(UsableItem item, bool unequipIfOccupied = false)
        {
            Equip(GetMainInput(), item, unequipIfOccupied);
        }


        public void Unequip(UsableItem item)
        {
            var slot = _inputSlots.Find(x => x.item == item);
            if(slot == null) {
                Debug.LogWarning("GamePlayer: Trying to unequip an item that wasn't equipped!");
                return;
            }

            slot.item = null;
            
            // notify everyone about the unequipped item
            CmdOnUnequip(item.gameObject);
        }

        // unequip an item assigned to a specific slot
        public void UnequipItemFrom(PlayerInput input)
        {
            var slot = FindInputSlot(input);
            if(slot.item == null || !IsItemEquipped(slot.item))
                return;

            Unequip(slot.item);
        }

        // unequips all equipped items
        public void UnequipAll()
        {
            foreach(var slot in _inputSlots) {
                if(slot.item != null) {
                    Unequip(slot.item);
                }
            }
        }

        // todo: this doesn't return client authority to the server!
        public void UnequipAllImmediate()
        {
            foreach(var slot in _inputSlots)
            {
                if(slot.item != null)
                {
                    OnUnequip(slot.item);
                }
            }
        }


        [Command] private void CmdOnEquip(GameObject item) {
            // assign authority to the equipped item
            item.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
            //item.GetComponent<NetworkTransform>().enabled = false;

            RpcOnEquip(item);
        }
        [ClientRpc] public void RpcOnEquip(GameObject item)
        {
            var usableItem = item.GetComponent<UsableItem>();
            PlayerInput input = null;
            
            if(usableItem == null) {
                Debug.LogError("GamePlayer.RpcOnEquip(): Can't find attached UsableItem component!");
                return;
            }

            // get the player input component from the input slot
            // if we're the local player
            if(isLocalPlayer) {
                var slot = FindInputSlot(usableItem);
                if(slot != null) {
                    input = slot.input;

                    usableItem.OnEquip(this, input);
                }
            }
                        
            OnEquip(input, usableItem);
        }

        [Command] private void CmdOnUnequip(GameObject item) {
            
            RpcOnUnequip(item);
            // revoke client authority after a set amount of seconds
            // todo:    there is no guarantee that our set timer 
            //          is enough for all clients to have run
            //          the item's deninitialize calls that may happen
            //          in UsableItem.OnUnequip. We should find a better
            //          way to revoke the clients authority w
            //StartCoroutine(RemoveClientAuthorityTimed());
        }
        [ClientRpc] public void RpcOnUnequip(GameObject item) {
            var usableItem = item.GetComponent<UsableItem>();

            if(usableItem == null) {
                Debug.LogError("GamePlayer.RpcOnEquip(): Can't find attached UsableItem component!");
                return;
            }

            // call the unequip hook of the concrete GamePlayer
            OnUnequip(usableItem);

            // if we're the local player make sure the item is properly unequipped
            // and stripped of its client authority
            if(isLocalPlayer) {
                usableItem.OnUnequip();
                usableItem.ReleaseAuthority();
            }
        }

        // register a new input slot with the base class
        protected void AddInputSlot(PlayerInput input)
        {
            if(IsInputSlotAssigned(input)) {
                Debug.LogError("GamePlayer: Trying to add an player input that is already assigned to an input slot!");
                return;
            }

            var inputSlot = new InputSlot();
            inputSlot.input = input;

            _inputSlots.Add(inputSlot);
        }

        protected abstract void OnEquip(PlayerInput input, UsableItem item);
        protected abstract void OnUnequip(UsableItem item);
        protected abstract PlayerInput GetMainInput();
    }

}