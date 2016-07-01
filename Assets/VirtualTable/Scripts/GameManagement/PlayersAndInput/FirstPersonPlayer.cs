using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.Characters.FirstPerson;

namespace CpvrLab.VirtualTable {

    [RequireComponent(typeof(FirstPersonPlayerInput))]
    public class FirstPersonPlayer : GamePlayer {

        [Header("First Person Properties")]
        [Range(0.5f, 3f)]
        public float pickupRange;
        public GameObject attachPoint;
        public Transform head;
        protected UsableItem _currentlyEquipped = null;
        protected MovableItem _currentlyHolding = null;
        protected FirstPersonPlayerInput _playerInput;
        
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            // temporary solution?
            GetComponent<CharacterController>().enabled = true;
            GetComponent<FirstPersonController>().enabled = true;
            
            _playerInput = GetComponent<FirstPersonPlayerInput>();
            // register the input slot with the base class
            AddInputSlot(_playerInput);

        }

        void Update()
        {
            if(!isLocalPlayer)
                return;

            if(_currentlyEquipped != null) {
                if(Input.GetKeyDown(KeyCode.E)) {
                    Unequip(_currentlyEquipped); // remove item from equipped input slot
                }
            }
            if(_currentlyHolding != null) {
                if(Input.GetKeyUp(KeyCode.E))
                    ReleaseMovableItem(_currentlyHolding);
            }
            else {
                // handle object pickups
                HandleItemInteractions();
            }
        }

        void HandleItemInteractions()
        {
            Ray ray = new Ray(head.position, head.forward);

            Debug.DrawLine(head.position, head.position + head.forward * pickupRange, Color.red);

            RaycastHit hit;
            if(!Physics.Raycast(ray, out hit, pickupRange))
                return;

            // todo: store the tags in some kind of const global variable!
            if(hit.transform.CompareTag("UsableItem")) {
                HandleUsableItem(hit);
            }
            
            // todo: store the tags in some kind of const global variable!
            if(hit.transform.CompareTag("MovableItem")) {
                HandleMovableItem(hit);
            }
        }

        void HandleUsableItem(RaycastHit hit)
        {
            // check if the object has the required UsableItem component attached
            var usableItem = hit.transform.GetComponent<UsableItem>();
            if(usableItem == null)
                return;            

            // Do we want to pick the item up?
            if(Input.GetKeyDown(KeyCode.E)) {
                Equip(usableItem);
            }
        }

        void HandleMovableItem(RaycastHit hit)
        {
            // check if the object has the required MovableItem component attached
            var movableItem = hit.transform.GetComponent<MovableItem>();
            if(movableItem == null)
                return;

            if(Input.GetKeyDown(KeyCode.E)) {
                GrabMovableItem(movableItem);
            }
        }

        void GrabMovableItem(MovableItem item)
        {
            // todo: grab the object
            //       best way would be to add the functionality into holdable item
            //       attach a fixed joint (or maybe a loose joint with the object hanging around?)
            //       anyway, what is   

        }

        void ReleaseMovableItem(MovableItem item)
        {

        }

        protected override PlayerInput GetMainInput()
        {
            return _playerInput;
        }

        protected override void OnEquip(PlayerInput input, UsableItem item)
        {
            // perform custom equip actions
            item.Attach(attachPoint); // attach the item to our "hand" object
            _currentlyEquipped = item;
        }
        
        protected override void OnUnequip(UsableItem item)
        {
            item.Detach(); // drop the item
            _currentlyEquipped = null;
        }
        
    } // class

} // namespace