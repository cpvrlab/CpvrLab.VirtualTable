using UnityEngine;
using System.Collections;
using Valve.VR;


namespace CpvrLab.VirtualTable {


    public delegate void InteractionEventHandler(PlayerInput input, UsableItem target);

    // todo:    separation of usable and movable items is tedious
    //          could this be simplified?
    // 
    // note:    at the moment only usable items work
    [RequireComponent(typeof(SphereCollider))]
    public class ViveInteractionController : MonoBehaviour {

        public event InteractionEventHandler UsableItemPickedUp;
        public event InteractionEventHandler UsableItemDropped;
        //public event InteractionEventHandler MovableItemPickedUp;
        //public event InteractionEventHandler MovableItemDropped;

        public PlayerInput input;
        public float pickupRadius = 0.1f;
        private SteamVR_Controller.Device _device;

        // currently holding an item?
        private bool holdingItem = false;
        private UsableItem _currentlyEquipped = null;


        // Use this for initialization
        void Start()
        {
            // todo: sanity checks
            var trackedObject = GetComponent<SteamVR_TrackedObject>();
            var index = (int)trackedObject.index;
            _device = SteamVR_Controller.Input(index);

            var col = GetComponent<SphereCollider>();
            col.radius = pickupRadius;
            col.isTrigger = true;
        }
        
        void OnTriggerEnter(Collider other)
        {
            if(other.attachedRigidbody == null)
                return;
            
            // todo: store these tags in a global common file as static const etc...
            bool usable = other.attachedRigidbody.CompareTag("UsableItem");
            bool movable = other.attachedRigidbody.CompareTag("MovableItem");
            
            if(!usable && !movable)
                return;

            if(holdingItem)
                return;
            
                        
            if(usable) {

                var item = other.attachedRigidbody.gameObject.GetComponent<UsableItem>();
                if (item != null)
                {
                    if (item.isInUse)
                        return;

                    _currentlyEquipped = item;
                    if (UsableItemPickedUp != null)
                        UsableItemPickedUp(input, _currentlyEquipped);

                    holdingItem = true;
                }
            }
        }

        void Update()
        {
            if(_device.GetPressDown(EVRButtonId.k_EButton_Grip)) {
                if(holdingItem) {
                    if(UsableItemDropped != null)
                        UsableItemDropped(input, _currentlyEquipped);

                    _currentlyEquipped = null;
                    holdingItem = false;
                }
            }
        }
    }
}