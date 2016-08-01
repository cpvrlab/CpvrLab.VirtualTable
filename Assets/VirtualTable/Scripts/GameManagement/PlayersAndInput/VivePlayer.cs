using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;

namespace CpvrLab.VirtualTable
{

    /// <summary>
    /// First implementation of a networked vive player.
    /// 
    /// todo:   this class needs a refactoring ASAP, it's only implemented as a proof ofconcept
    ///         and is poorly designed in some places.
    /// </summary>
    public class VivePlayer : GamePlayer
    {

        [Header("Vive Player Properties")]
        public bool isRightHanded = true;

        public GameObject head;
        public GameObject leftController;
        public GameObject rightController;

        // temporary only for testing. we'll use a better solution later
        public GameObject tempCameraRig;

        public GameObject hmd = null;


        // unsure if these variables are necessary 
        protected ViveInteractionController _leftInteraction;
        protected ViveInteractionController _rightInteraction;

        protected VivePlayerInput _leftInput;
        protected VivePlayerInput _rightInput;
        
                
        public override void OnStartClient()
        {
            base.OnStartClient();

            AddAttachmentSlot(leftController);
            AddAttachmentSlot(rightController);
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            tempCameraRig.SetActive(true);

            var controllerManager = FindObjectOfType<SteamVR_ControllerManager>();

            var left = controllerManager.left;
            var right = controllerManager.right;
            var leftTrackedObj = left.GetComponent<SteamVR_TrackedObject>();
            var rightTrackedObj = right.GetComponent<SteamVR_TrackedObject>();

            // add interaction controllers
            // left
            _leftInteraction = left.GetComponent<ViveInteractionController>();
            if (_leftInteraction == null)
                _leftInteraction = left.AddComponent<ViveInteractionController>();
            // right
            _rightInteraction = right.GetComponent<ViveInteractionController>();
            if (_rightInteraction == null)
                _rightInteraction = right.AddComponent<ViveInteractionController>();

            // add vive player input
            // left
            _leftInput = left.GetComponent<VivePlayerInput>();
            if (_leftInput == null)
                _leftInput = left.AddComponent<VivePlayerInput>();

            // right
            _rightInput = right.GetComponent<VivePlayerInput>();
            if (_rightInput == null)
                _rightInput = right.AddComponent<VivePlayerInput>();

            // Update the input slots added in OnStartClient
            FindAttachmentSlot(leftController).input = _leftInput;
            FindAttachmentSlot(rightController).input = _rightInput;

            // the interactin controllers also need the player input
            _leftInteraction.input = _leftInput;
            _rightInteraction.input = _rightInput;

            // add the tracked object to the VivePlayerInput
            // for it to know where to get input from
            _leftInput.trackedObj = leftTrackedObj;
            _rightInput.trackedObj = rightTrackedObj;

            // connect pickup and drop delegates of the interaction controllers
            // to be notified when we should pick up a usable item
            _leftInteraction.UsableItemPickedUp += ItemPickedUp;
            _rightInteraction.UsableItemPickedUp += ItemPickedUp;
            _leftInteraction.UsableItemDropped += ItemDropped;
            _rightInteraction.UsableItemDropped += ItemDropped;

            _leftInteraction.MovableItemPickedUp += MovableItemPickedUp;
            _rightInteraction.MovableItemPickedUp += MovableItemPickedUp;
            _leftInteraction.MovableItemDropped += MovableItemDropped;
            _rightInteraction.MovableItemDropped += MovableItemDropped;


            // finally we want to find the gameobject representation
            // of the actual vive HMD
            // todo: can we do this a bit cleaner?
            

        }

        /// <summary>
        /// The local player makes sure to update the three gameObjects
        /// that represent the head and hands over the network
        /// </summary>
        void Update()
        {
            if (!isLocalPlayer)
                return;

            head.transform.position = hmd.transform.position;
            head.transform.rotation = hmd.transform.rotation;

            leftController.transform.position = _leftInteraction.transform.position;
            leftController.transform.rotation = _leftInteraction.transform.rotation;

            rightController.transform.position = _rightInteraction.transform.position;
            rightController.transform.rotation = _rightInteraction.transform.rotation;

        }

        /// <summary>
        /// Called when ever one of our controllers picks up a UsableItem
        /// </summary>
        /// <param name="input"></param>
        /// <param name="item"></param>
        void ItemPickedUp(PlayerInput input, UsableItem item)
        {
            Equip(input, item, false);
        }
        
        /// <summary>
        /// Called when ever one of our controllers drops a UsableItem
        /// </summary>
        /// <param name="input"></param>
        /// <param name="item"></param>
        void ItemDropped(PlayerInput input, UsableItem item)
        {
            Unequip(item);
        }

        void MovableItemPickedUp(PlayerInput input, MovableItem item)
        {
            Debug.Log("Grabbing " + GetSlotIndex(FindAttachmentSlot(input)) + " " + item.name);
            CmdGrabMovableItem(item.gameObject, GetSlotIndex(FindAttachmentSlot(input)));
        }
        void MovableItemDropped(PlayerInput input, MovableItem item)
        {
            Debug.Log("Releasing");
            CmdReleaseMovableItem(item.gameObject, GetSlotIndex(FindAttachmentSlot(input)));
        }

        protected override PlayerInput GetMainInput()
        {
            if (isRightHanded)
                return _rightInput;
            else
                return _leftInput;
        }

        protected override void OnEquip(AttachmentSlot slot)
        {
            slot.item.transform.localRotation = Quaternion.Euler(90, 0, 0);       
        }

        protected override void OnUnequip(UsableItem item)
        {
        }

    } // class

} // namespace