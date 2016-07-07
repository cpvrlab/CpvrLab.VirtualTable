using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

namespace CpvrLab.VirtualTable {
    
    /// <summary>
    /// Abstract base class for all game players. It is responsible for interacting with UsableItems
    /// and routing player input to the necessary receivers.
    /// 
    /// todo:   We should probably rename this class to better reflect that this is the main representation
    ///         of a player over the network.
    ///         
    /// todo:   We don't keep track about attachment slots on the server currently. This is because we 
    ///         only have a host option at this point and not a standalone server. If we ever wanted 
    ///         to add dedicated server support we'd need to change that. Maybe we should be making these
    ///         changes now to make it easier in the future.
    ///         
    /// todo:   IMPORTANT! attachment slos aren't synced over the network. So when a new client connects late
    ///         he won't be able to know what items are equipped on which player. This also means that
    ///         the equipped items won't be a child of that gameplayer and that the item itself also doesn't 
    ///         know it is owned by a player. This can lead to other players trying to pick up items already in use.
    /// </summary>
    public abstract class GamePlayer : NetworkBehaviour {
        
        /// <summary>
        /// defines an attachment slot where UsableItems can be attached to
        /// later by the concrete player implementation.
        /// </summary>
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
        protected PlayerModel _playerModelInstance = null;
        
        
        public override void OnStartServer()
        {
            // add ourselves to the current game manager server instance
            GameManager.instance.AddPlayer(this);
        }


        /// <summary>
        /// Here we instantiate player models for this GamePlayer. Both remote and local instances, which may have
        /// different player model representations. 
        /// 
        /// todo:   Can we rely on Start being called AFTER localplayerstart? Or more specific can we rely on 
        ///         isLocalPlayer to contain the correct value every time? I'm not too sure this is the right
        ///         approach and we might need our own implementation for this to work properly. For now
        ///         this does the job and I'll just leave it be.
        /// </summary>
        public void Start()
        {
            if (isLocalPlayer && playerModels.Count > _localPlayerModel)
                SetPlayerModel(_localPlayerModel);
            else if (playerModels.Count > _remotePlayerModel)
                SetPlayerModel(_remotePlayerModel);
        }

        protected void SetPlayerModel(int index)
        {
            if (index < 0 || playerModels.Count <= index)
                return;

            // remove current model
            DestroyPlayerModel();

            _playerModelInstance = Instantiate(playerModels[index], transform.position, transform.rotation) as PlayerModel;
            _playerModelInstance.InitializeModel(this);
            _playerModelInstance.playerText.text = displayName;
        }

        protected void DestroyPlayerModel()
        {
            if (_playerModelInstance == null)
                return;

            Destroy(_playerModelInstance.gameObject);
            _playerModelInstance = null;
        }

        [Client] protected void NextLocalModel()
        {
            if (!isLocalPlayer)
                return;

            _localPlayerModel++;
            _localPlayerModel %= playerModels.Count;

            SetPlayerModel(_localPlayerModel);
        }

        [Client] protected void NextRemoteModel()
        {
            if (!isLocalPlayer)
                return;

            _remotePlayerModel++;
            _remotePlayerModel %= playerModels.Count;

            CmdSetRemoteModel(_remotePlayerModel);
        }
        [Command] protected void CmdSetRemoteModel(int index) { RpcSetRemoteModel(index); }
        [ClientRpc] protected void RpcSetRemoteModel(int index) { _remotePlayerModel = index; SetPlayerModel(index); }

        public override void OnNetworkDestroy()
        {
            if (!isServer)
            {
                UnequipAllLocal();
                DestroyPlayerModel();
            }
            base.OnNetworkDestroy();
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
        /// Can be called on server or local player to equip an item
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

        /// <summary>
        /// Equip an item to the attachmentslot associated with the PlayerInput "input"
        /// </summary>
        /// <param name="input"></param>
        /// <param name="item"></param>
        /// <param name="unequipIfOccupied"></param>
        [Client] protected void Equip(PlayerInput input, UsableItem item, bool unequipIfOccupied = false)
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
                //       all clients?
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
            // todo: make sure that the stuff said in the note above is correct.
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

            UnequipLocal(item);
        }

        protected void UnequipAllLocal()
        {
            foreach (var slot in _attachmentSlots)
                UnequipLocal(slot.item);
        }

        protected void UnequipLocal(UsableItem item)
        {
            var slot = FindAttachmentSlot(item);
            if (slot == null)
                return;



            slot.item = null;

            Debug.Log("UNEQUIPPING item");

            item.Detach();
            item.ClearOwner();
            item.ReleaseAuthority();
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

        //public override bool OnSerialize(NetworkWriter writer, bool initialState)
        //{
        //    bool wroteSync = base.OnSerialize(writer, initialState);

        //    writer.Write(synctest);

        //    return wroteSync;
        //}

        //public override void OnDeserialize(NetworkReader reader, bool initialState)
        //{
        //    base.OnDeserialize(reader, initialState);

        //    synctest = reader.ReadInt32();
        //    Debug.Log("Deserialize " + synctest);
        //}


        // todo: these functions don't need to be abstract anymore
        protected virtual void OnEquip(AttachmentSlot slot) { }
        protected virtual void OnUnequip(UsableItem item) { }
        protected abstract PlayerInput GetMainInput();
    }

}