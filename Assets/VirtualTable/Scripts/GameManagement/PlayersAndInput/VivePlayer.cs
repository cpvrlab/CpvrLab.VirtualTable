using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CpvrLab.VirtualTable
{


    public class VivePlayer : GamePlayer
    {

        [Header("Vive Player Properties")]
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
                EquipItem(_leftInput, target.GetComponent<UsableItem>());
            else
                EquipItem(_rightInput, target.GetComponent<UsableItem>());
        }

        void ItemDropped(object sender, GameObject target)
        {
            UnequipItem(target.GetComponent<UsableItem>());
        }


    } // class

} // namespace