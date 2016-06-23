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
        public event InteractionEventHandler MovableItemPickedUp;
        public event InteractionEventHandler MovableItemDropped;
        
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
            bool moveable = other.attachedRigidbody.CompareTag("MovableItem");
            
            if(!usable && !moveable)
                return;

            if(holdingItem)
                return;
            
                        
            if(usable) {
                _currentlyEquipped = other.attachedRigidbody.gameObject;

                if(UsableItemPickedUp != null)
                    UsableItemPickedUp(this, _currentlyEquipped);

                // add the item to our player
                _currentlyEquipped.transform.SetParent(transform, false);
                _currentlyEquipped.transform.localPosition = Vector3.zero;
                _currentlyEquipped.transform.localRotation = Quaternion.Euler(90, 0, 0);
                _currentlyEquipped.GetComponent<Rigidbody>().isKinematic = true;

            }
        }

        void Update()
        {
            if(_device.GetPressDown(EVRButtonId.k_EButton_Grip)) {
                if(holdingItem) {
                    _currentlyEquipped.transform.SetParent(null, true);
                    _currentlyEquipped.transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;

                    if(UsableItemDropped != null)
                        UsableItemDropped(this, _currentlyEquipped);

                    _currentlyEquipped = null;
                }
            }
        }
    }
}