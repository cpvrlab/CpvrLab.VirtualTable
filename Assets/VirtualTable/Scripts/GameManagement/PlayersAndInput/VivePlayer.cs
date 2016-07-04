using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;

namespace CpvrLab.VirtualTable
{

    /// <summary>
    /// Note: this class needs a refactoring ASAP, it's only implemented as a proof ofconcept
    /// and is poorly designt in some places.
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

        protected GameObject hmd = null;


        // unsure if these variables are necessary 
        protected ViveInteractionController _leftInteraction;
        protected ViveInteractionController _rightInteraction;

        protected VivePlayerInput _leftInput;
        protected VivePlayerInput _rightInput;

        protected UsableItem equipped;

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

            // add input slots to base class
            AddInputSlot(_leftInput);
            AddInputSlot(_rightInput);

            _leftInput.trackedObj = leftTrackedObj;
            _rightInput.trackedObj = rightTrackedObj;

            _leftInteraction.UsableItemPickedUp += ItemPickedUp;
            _rightInteraction.UsableItemPickedUp += ItemPickedUp;
            _leftInteraction.UsableItemDropped += ItemDropped;
            _rightInteraction.UsableItemDropped += ItemDropped;


            var trackedObjects = FindObjectsOfType<SteamVR_TrackedObject>();
            foreach (var obj in trackedObjects)
            {
                if (obj.index == SteamVR_TrackedObject.EIndex.Hmd)
                {
                    hmd = obj.gameObject;
                }
            }

            if (hmd == null)
                Debug.LogError("VivePlayer: couldn't find a hmd for the local player");

        }

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

        void ItemPickedUp(object sender, GameObject target)
        {
            equipped = target.GetComponent<UsableItem>();
            // todo:     this implementation is crap. but at the moment I'm still unsure if we even need to tell
            //           the base GamePlayer class about our equipped items or not
            if (_rightInteraction.Equals(sender))
            {
                Equip(_rightInput, equipped);
            }
        }
        

        void ItemDropped(object sender, GameObject target)
        {
            Unequip(equipped);
            equipped = null;
        }

        protected override PlayerInput GetMainInput()
        {
            if (isRightHanded)
                return _rightInput;
            else
                return _leftInput;
        }

        protected override void OnEquip(PlayerInput input, UsableItem item)
        {
            // todo:    I reall don't like this implementation, review this after having some games working.


            
            item.Attach(rightController);
            
        }

        protected override void OnUnequip(UsableItem item)
        {
            Debug.Log("detaching");
            item.Detach();
        }
    } // class

} // namespace