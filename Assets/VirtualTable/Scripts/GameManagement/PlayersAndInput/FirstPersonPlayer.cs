using UnityEngine;
using System.Collections;
using System;

namespace CpvrLab.VirtualTable {

    [RequireComponent(typeof(FirstPersonPlayerInput))]
    public class FirstPersonPlayer : GamePlayer {

        [Header("First Person Properties")]
        [Range(0.5f, 3f)]
        public float pickupRange;
        public GameObject attachPoint;
        protected Transform head;
        protected UsableItem _currentlyEquipped = null;
        protected FirstPersonPlayerInput _playerInput;

        protected override void Start()
        {
            base.Start();
            head = Camera.main.transform;
            _playerInput = GetComponent<FirstPersonPlayerInput>();
            // register the input slot with the base class
            AddInputSlot(_playerInput);
        }

        void Update()
        {
            if(_currentlyEquipped != null) {
                if(Input.GetKeyDown(KeyCode.E)) {
                    UnequipItem(_currentlyEquipped); // remove item from equipped input slot
                    _currentlyEquipped.Detach(); // drop the item
                    _currentlyEquipped = null;
                }
            }
            else {
                // pickup objects
                HandlePickup();
            }
        }

        void HandlePickup()
        {
            Ray ray = new Ray(head.position, head.forward);

            Debug.DrawLine(head.position, head.position + head.forward * pickupRange, Color.red);

            RaycastHit hit;
            if(!Physics.Raycast(ray, out hit, pickupRange))
                return;

            // todo: store the tags in some kind of const global variable!
            // check if the hit object is usable
            if(!hit.transform.CompareTag("UsableItem"))
                return;

            // check if the object has the required UsableItem component attached
            var usableItem = hit.transform.GetComponent<UsableItem>();
            if(usableItem == null)
                return;

            // finally check if we should pick the object up
            if(Input.GetKeyDown(KeyCode.E)) {
                EquipItem(_playerInput, usableItem); // add the item to our input slot
                usableItem.Attach(attachPoint); // attach the item to our "hand" object
                _currentlyEquipped = usableItem;
            }
        }
        

    } // class

} // namespace