using UnityEngine;
using System.Collections;

namespace CpvrLab.VirtualTable {
    public class UsableItem : MonoBehaviour {

        public Transform attachPoint;
        protected PlayerInput _input;
        protected Transform _prevParent = null;
        
        public void Attach(GameObject attach)
        {
            _prevParent = transform.parent;

            transform.parent = attach.transform;
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;

            // todo: not sure if it's a good idea to handle this here.
            // "disable" rigidbody by setting it to kinematic
            var rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        public void Detach()
        {
            transform.parent = _prevParent;
            var rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
        }

        public virtual void OnEquip(PlayerInput input) {
            _input = input;
        }
        public virtual void OnUnequip() {
            _input = null;
        }
    }
}