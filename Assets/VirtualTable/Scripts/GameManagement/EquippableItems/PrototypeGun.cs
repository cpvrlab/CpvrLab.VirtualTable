using UnityEngine;
using System.Collections;

namespace CpvrLab.VirtualTable {

    public class PrototypeGun : EquippableItem {

        public Transform trigger;
        public float triggerAnglePressed;
        public float triggerAngleReleased;

        void Update()
        {
            if(_input == null)
                return;

            // todo: this seems tedious, can't we just subscribe to buttons and receive input?
            if(_input.GetActionDown(PlayerInput.ActionCode.Button0))
                Debug.Log("boom");

            float factor = _input.GetAxis(PlayerInput.AxisCode.Axis0);
            float angle = Mathf.Lerp(triggerAngleReleased, triggerAnglePressed, factor);
            trigger.localRotation = Quaternion.AngleAxis(angle, Vector3.right);
        }
    }

}