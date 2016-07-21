using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.Characters.FirstPerson;

namespace CpvrLab.VirtualTable {

    /// <summary>
    /// Prototype implementation of a FirstPersonPlayer. This player is controlled by mouse and 
    /// keyboard like a traditional first person game. 
    /// </summary>
    [RequireComponent(typeof(FirstPersonPlayerInput))]
    public class FirstPersonPlayer : GamePlayer {

        [Header("First Person Properties")]
        [Range(0.5f, 3f)]
        public float pickupRange;
        public GameObject attachPoint;
        public Transform head;
        public GameObject fpsGUIPrefab;
        protected GameObject _fpsGUIInstance;

        protected UsableItem _currentlyEquipped = null;
        protected MovableItem _currentlyHolding = null;
        protected FirstPersonPlayerInput _playerInput;

        public override void OnStartClient()
        {
            base.OnStartClient();

            // add attachment slots on all clients
            AddAttachmentSlot(attachPoint);
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            // temporary solution?
            GetComponent<CharacterController>().enabled = true;
            GetComponent<FirstPersonController>().enabled = true;
            head.GetComponent<AudioListener>().enabled = true;
            
            _playerInput = GetComponent<FirstPersonPlayerInput>();
            
            // add the player input component to our attachment slot
            // todo: this implementation doesn't seem that good
            //       although we don't need a sanity check here it still feels dangerous and wrong.
            FindAttachmentSlot(attachPoint).input = _playerInput;

            // instantiate the GUI
            _fpsGUIInstance = Instantiate(fpsGUIPrefab);
        }

        void OnDestroy()
        {
            Destroy(_fpsGUIInstance);
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

            // test for model switching
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                NextLocalModel();
                NextRemoteModel();
            }

            //if(_currentlyHolding != null)
            //{
                //var rb1 = attachPoint.GetComponent<Rigidbody>();
                //var rb2 = _currentlyHolding.GetComponent<Rigidbody>();
                //Debug.Log("AttachPoint: " + rb1.velocity + " " + rb1.angularVelocity + "; Item: " + rb2.velocity + " " + rb2.angularVelocity);
            //}
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
            _currentlyHolding = item;
            //item.Attach(attachPoint.GetComponent<Rigidbody>());
            CmdGrabMovableItem(item.gameObject, 0);
        }

        void ReleaseMovableItem(MovableItem item)
        {
            _currentlyHolding = null;
            //item.Detach();
            CmdReleaseMovableItem(item.gameObject, 0);
        }

        protected override PlayerInput GetMainInput()
        {
            return _playerInput;
        }

        protected override void OnEquip(AttachmentSlot slot)
        {
            _currentlyEquipped = slot.item;
        }
        
        protected override void OnUnequip(UsableItem item)
        {
            _currentlyEquipped = null;
        }
        
    } // class

} // namespace