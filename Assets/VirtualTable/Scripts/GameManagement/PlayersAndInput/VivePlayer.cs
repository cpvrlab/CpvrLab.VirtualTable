using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CpvrLab.VirtualTable
{


    public class VivePlayer : GamePlayer
    {

        [Header("Vive Player Properties")]
        public bool isRightHanded = true;

        // unsure if these variables are necessary 
        protected ViveInteractionController _leftInteraction;
        protected ViveInteractionController _rightInteraction;

        protected VivePlayerInput _leftInput;
        protected VivePlayerInput _rightInput;

        protected override void Start()
        {
            base.Start();

            var controllerManager = GetComponent<SteamVR_ControllerManager>();

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
        }

        void ItemPickedUp(object sender, GameObject target)
        {
            // todo:     this implementation is crap. but at the moment I'm still unsure if we even need to tell
            //           the base GamePlayer class about our equipped items or not
            if (_leftInteraction.Equals(sender))
                Equip(_leftInput, target.GetComponent<UsableItem>());
            else
                Equip(_rightInput, target.GetComponent<UsableItem>());
        }

        void ItemDropped(object sender, GameObject target)
        {
            Unequip(target.GetComponent<UsableItem>());
        }
        
        protected override PlayerInput GetMainInput()
        {
            if(isRightHanded)
                return _rightInput;
            else
                return _leftInput;
        }

        protected override void OnEquip(PlayerInput input, UsableItem item)
        {
            // todo:    I reall don't like this implementation, review this after having some games working.
            input.gameObject.GetComponent<ViveInteractionController>().AttachItem(item);
        }

        protected override void OnUnequip(PlayerInput input, UsableItem item)
        {
            Debug.Log("OnUnequip");
            input.gameObject.GetComponent<ViveInteractionController>().DetachItem(item);
        }
    } // class

} // namespace