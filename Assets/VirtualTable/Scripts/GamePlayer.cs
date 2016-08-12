using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System;

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

        public static GamePlayer localPlayer = null;
        public static Action<GamePlayer> OnLocalPlayerCreated;

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
        protected List<AttachmentSlot> _attachmentSlots = new List<AttachmentSlot>();
        
        [HideInInspector]
        [SyncVar] public string displayName = "player";
        [HideInInspector]
        [SyncVar(hook= "IsObserverHook")] public bool isObserver = false;



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

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            if (OnLocalPlayerCreated != null)
                OnLocalPlayerCreated(this);
            localPlayer = this;
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
            
            _playerModelInstance.playerText.enabled = !isLocalPlayer;
            
        }

        void IsObserverHook(bool val)
        {
            isObserver = val;
            // hide player model if we're in observer mode
            if(_playerModelInstance != null)
                _playerModelInstance.gameObject.SetActive(!val);
            OnObserverStateChanged(val);
        }

        protected virtual void OnObserverStateChanged(bool val) { }

        protected void DestroyPlayerModel()
        {
            if (_playerModelInstance == null)
                return;
            
            Destroy(_playerModelInstance.gameObject);
            _playerModelInstance = null;
        }

        protected virtual void OnDestroy()
        {
            DestroyPlayerModel();
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
            UnequipAllLocal();
            GameManager.instance.RemovePlayer(this);
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
                RpcEquipToMain(item.gameObject, unequipIfOccupied);
            }
        }

        /// <summary>
        /// todo:   switch this stuff around. An equip call should probably start on the server. 
        ///         So at the moment when the server wants to equip an item to a specific player
        ///         he has to trigger that player to call Equip locally. Which seems like unnecessary
        ///         round trips. 
        /// </summary>
        /// <param name="item"></param>
        [ClientRpc] protected void RpcEquipToMain(GameObject itemGO, bool unequipIfOccupied)
        {
            if (!isLocalPlayer)
                return;

            var item = itemGO.GetComponent<UsableItem>();
            if (item == null)
                return;

            Equip(GetMainInput(), item, unequipIfOccupied);
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
        /// Note: Bad design!
        /// This function is currently used by UsableItem to equip itself to late connecting clients if a player
        /// on the server has an item equipped.
        /// 
        /// todo: find a better solution for this.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="attachPoint"></param>
        public void EquipLocal(UsableItem item, GameObject attachPoint)
        {
            var slot = FindAttachmentSlot(attachPoint);

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
            var slot = FindAttachmentSlot(item);
            if (slot == null)
            {
                Debug.LogWarning("GamePlayer: Trying to unequip an item that wasn't equipped!");
                return;
            }

            // notify everyone about the unequipped item
            CmdOnUnequip(item.gameObject);
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
            if (item == null || slot == null)
                return;


            slot.item = null;
            
            item.Detach();
            item.ClearOwner();
            item.ReleaseAuthority();
        }
        
        [Command] protected void CmdGrabMovableItem(GameObject itemGO, int slotIndex)
        {
            // todo: we should keep track of movable items on both server and client
            RpcGrabMovableItem(itemGO, slotIndex);
        }
        [ClientRpc] protected void RpcGrabMovableItem(GameObject itemGO, int slotIndex)
        {
            var item = itemGO.GetComponent<MovableItem>();
            item.Attach(GetAttachmentSlot(slotIndex).attachPoint.GetComponent<Rigidbody>());
        }
        [Command] protected void CmdReleaseMovableItem(GameObject itemGO, int slotIndex)
        {
            RpcReleaseMovableItem(itemGO, slotIndex);
        }
        [ClientRpc] protected void RpcReleaseMovableItem(GameObject itemGO, int slotIndex)
        {
            var item = itemGO.GetComponent<MovableItem>();
            item.Detach();
        }

        // register a new input slot with the base class
        [Client] protected void AddAttachmentSlot(GameObject attachPoint, PlayerInput input = null)
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


        /// <summary>
        /// Note:   we override on serialize and deserialize here to 
        ///         be able to sync attachment slot item states.
        ///         this means we also need to sync the sync vars by
        ///         ourselves. 
        ///         
        /// todo:   optimize the syncing of sync vars by utilizing 
        ///         the dirty bits correctly.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="initialState"></param>
        /// <returns></returns>
        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            bool wroteSync = base.OnSerialize(writer, initialState);

            if (initialState)
            {
                // always write initial state, no dirty bits
            }
            else if (syncVarDirtyBits == 0)
            {
                writer.WritePackedUInt32(0);
                return wroteSync;
            }
            else
            {
                // dirty bits
                writer.WritePackedUInt32(1);
            }

            writer.Write(displayName);
            writer.Write(isObserver);


            // serialize currently equipped items
            for (int i = 0; i < _attachmentSlots.Count; i++)
            {
                GameObject go = (_attachmentSlots[i].item == null) ? null : _attachmentSlots[i].item.gameObject;
                writer.Write(go);
            }

            return true;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            base.OnDeserialize(reader, initialState);

            if (isServer && NetworkServer.localClientActive)
                return;

            if (!initialState)
            {
                if (reader.ReadPackedUInt32() == 0)
                    return;
            }
            

            displayName = reader.ReadString();
            isObserver = reader.ReadBoolean();

            // serialize currently equipped items
            for (int i = 0; i < _attachmentSlots.Count; i++)
            {
                var itemGO = reader.ReadGameObject();
                var item = (itemGO != null) ? itemGO.GetComponent<UsableItem>() : null;

                //Debug.Log("item " + item);
                if (item != null)
                {
                    var slot = _attachmentSlots[i];
                    slot.item = item;
                    slot.item.AssignOwner(this, null);
                    slot.item.Attach(slot.attachPoint);

                    OnEquip(slot);
                }

            }
        }
    }

}