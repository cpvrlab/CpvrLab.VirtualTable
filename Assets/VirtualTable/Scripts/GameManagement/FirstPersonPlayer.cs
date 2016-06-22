using UnityEngine;
using System.Collections;
using System;

namespace CpvrLab.VirtualTable {

    [RequireComponent(typeof(FirstPersonPlayerInput))]
    public class FirstPersonPlayer : GamePlayer {

        [Header("First Person Properties")]
        [Range(0.5f, 3f)]
        public float pickupRange;
        public Transform attachPoint;
        protected Transform head;

        protected override void Start()
        {
            base.Start();
            head = Camera.main.transform;
        }

        void Update()
        {
            // pickup objects
            HandlePickup();
        }

        void HandlePickup()
        {
            Ray ray = new Ray(head.position, head.forward);

            Debug.DrawLine(head.position, head.position + head.forward * pickupRange, Color.red);

            RaycastHit hit;
            if(!Physics.Raycast(ray, out hit, pickupRange))
                return;

            // todo: store the tags in some kind of const global variable!
            // check if the hit object is equippable
            if(!hit.transform.CompareTag("Equippable"))
                return;

            // check if the object has the required equippableitem component attached
            var equippable = hit.transform.GetComponent<EquippableItem>();
            if(equippable == null)
                return;

            // finally check if we should pick the object up
            if(Input.GetKeyDown(KeyCode.E)) {
                // call base class equip so that the item receives input
                EquipItem(equippable);

                // add the item to our player
                hit.transform.SetParent(attachPoint, false);
                hit.transform.localPosition = Vector3.zero;
                hit.transform.localRotation = Quaternion.identity;
                hit.transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    } // class

} // namespace