using UnityEngine;
using System.Collections;
using Valve.VR;


namespace CpvrLab.VirtualTable {


    public delegate void InteractionEventHandler(object sender, GameObject target);

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
        
        public float pickupRadius = 0.1f;
        private SteamVR_Controller.Device _device;

        // currently holding an item?
        private bool holdingItem { get { return _currentlyEquipped != null; } }
        private GameObject _currentlyEquipped;


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
                // notify listeners about the pickup
                // this will trigger a AttachItem to be called by VivePlayer
                // because the item will be registered with the GamePlayer base class
                if(UsableItemPickedUp != null)
                    UsableItemPickedUp(this, other.attachedRigidbody.gameObject);
            }
        }

        void Update()
        {
            if(_device.GetPressDown(EVRButtonId.k_EButton_Grip)) {
                if(holdingItem) {
                    // notify listeners about the pickup
                    // this will trigger a Unattach to be called by VivePlayer
                    // because the item will be registered with the GamePlayer base class
                    if(UsableItemDropped != null)
                        UsableItemDropped(this, _currentlyEquipped);
                }
            }
        }

        public void AttachItem(UsableItem item)
        {
            // attach the item to this gameObject
            item.Attach(gameObject);
            item.transform.localRotation = Quaternion.Euler(90, 0, 0);
            _currentlyEquipped = item.gameObject;
        }

        public void DetachItem(UsableItem item)
        {
            item.Detach();
            _currentlyEquipped = null;
        }
    }
}