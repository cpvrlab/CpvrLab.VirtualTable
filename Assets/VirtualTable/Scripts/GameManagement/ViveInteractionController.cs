using UnityEngine;
using System.Collections;
using Valve.VR;


namespace CpvrLab.VirtualTable {


    public delegate void InteractionEventHandler(object sender, GameObject target);

    // todo:    separation of equippable and holdable items is tedious
    //          could this be simplified?
    // 
    // note:    at the moment only equippable items work
    [RequireComponent(typeof(SphereCollider))]
    public class ViveInteractionController : MonoBehaviour {

        public event InteractionEventHandler EquippableItemPickedUp;
        public event InteractionEventHandler EquippableItemDropped;
        public event InteractionEventHandler HoldableItemPickedUp;
        public event InteractionEventHandler HoldableItemDropped;
        
        public float pickupRadius = 0.2f;
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
            bool equippable = other.attachedRigidbody.CompareTag("Equippable");
            bool holdable = other.attachedRigidbody.CompareTag("Holdable");
            
            if(!equippable && !holdable)
                return;

            if(holdingItem)
                return;
            
                        
            if(equippable) {
                _currentlyEquipped = other.attachedRigidbody.gameObject;

                if(EquippableItemPickedUp != null)
                    EquippableItemPickedUp(this, _currentlyEquipped);

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

                    if(EquippableItemDropped != null)
                        EquippableItemDropped(this, _currentlyEquipped);

                    _currentlyEquipped = null;
                }
            }
        }
    }
}