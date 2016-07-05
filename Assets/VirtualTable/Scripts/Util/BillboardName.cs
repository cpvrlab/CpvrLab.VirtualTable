using UnityEngine;
using System.Collections;

namespace CpvrLab.VirtualTable
{


    public class BillboardName : MonoBehaviour
    {

        public bool inheritParentRotation = false;
        private Vector3 _offset;

        void Start()
        {
            if (!inheritParentRotation && transform.parent != null)
                _offset = transform.position - transform.parent.position;
        }

        void Update()
        {
            if (Camera.main == null)
                return;

            // ignore rotation of parent
            if (!inheritParentRotation)
            {
                transform.position = transform.parent.position + _offset;
            }

            var target = Camera.main.transform.position;
            target.y = transform.position.y;
            transform.LookAt(target);
            transform.Rotate(0, 180, 0);
        }
    }

}