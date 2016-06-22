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
            Ray ray = new Ray(head.position, head.forward);
            Debug.DrawLine(head.position, head.position + pickupRange * head.forward, Color.red, 0.1f);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, pickupRange)) {
                // todo: store the tags in some kind of const global variable!
                if(hit.transform.CompareTag("Equippable")) {
                    Debug.Log("PICKUP");
                    if(Input.GetKeyDown(KeyCode.E)) {
                        hit.transform.SetParent(attachPoint, false);
                        hit.transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                        Debug.Log("PICKUP BLEH");
                    }


                }
            }

        }
    }

}