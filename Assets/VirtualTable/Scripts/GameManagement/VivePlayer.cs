using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CpvrLab.VirtualTable {

    [RequireComponent(typeof(VivePlayerInput))]
    public class VivePlayer : GamePlayer {

        [Header("Vive Player Properties")]
        protected GameObject _currentlyEquipped = null;
        protected SteamVR_TrackedObject _left;
        protected SteamVR_TrackedObject _right;

        protected override void Start()
        {
            base.Start();


            var controllerManager = GetComponent<SteamVR_ControllerManager>();
            

            var viveInput = (VivePlayerInput)_playerInput;
            viveInput.trackedObj = controllerManager.right.GetComponent<SteamVR_TrackedObject>();
            // add interaction controllers and subscribe to necessary events
            // left
            var interactionController = controllerManager.left.GetComponent<ViveInteractionController>();
            if(interactionController == null)
                interactionController = controllerManager.left.AddComponent<ViveInteractionController>();

            interactionController.EquippableItemPickedUp += EquippableItemPickedUp;
            interactionController.EquippableItemDropped += EquippableItemPickedDropped;

            // right
            interactionController = controllerManager.right.GetComponent<ViveInteractionController>();
            if(interactionController == null)
                interactionController = controllerManager.right.AddComponent<ViveInteractionController>();

            interactionController.EquippableItemPickedUp += EquippableItemPickedUp;
            interactionController.EquippableItemDropped += EquippableItemPickedDropped;
        }

        void EquippableItemPickedUp(object sender, GameObject item)
        {
            EquipItem(item.GetComponent<EquippableItem>());
        }
        void EquippableItemPickedDropped(object sender, GameObject item)
        {
            UnequipItem(item.GetComponent<EquippableItem>());
        }
        
    } // class

} // namespace