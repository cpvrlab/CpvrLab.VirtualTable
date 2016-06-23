using UnityEngine;
using System.Collections.Generic;
using System;

namespace CpvrLab.VirtualTable {

    public class FirstPersonPlayerInput : PlayerInput {

        private Dictionary<ActionCode, KeyCode> _actionMapping;
        private Dictionary<AxisCode, string> _axisMapping;
        private Camera _camera;

        void Awake()
        {
            _actionMapping = new Dictionary<ActionCode, KeyCode>();
            _axisMapping = new Dictionary<AxisCode, string>();

            // map key codes to our internal button codes
            _actionMapping.Add(ActionCode.Button0, KeyCode.Mouse0);
            _actionMapping.Add(ActionCode.Button1, KeyCode.Mouse1);

            _camera = Camera.main;
        }

        public override bool GetAction(ActionCode ac)
        {
            if(!_actionMapping.ContainsKey(ac))
                return false;

            return Input.GetKey(_actionMapping[ac]);
        }

        public override bool GetActionDown(ActionCode ac)
        {
            if(!_actionMapping.ContainsKey(ac))
                return false;

            return Input.GetKeyDown(_actionMapping[ac]);
        }

        public override bool GetActionUp(ActionCode ac)
        {
            if(!_actionMapping.ContainsKey(ac))
                return false;

            return Input.GetKeyUp(_actionMapping[ac]);
        }

        public override float GetAxis(AxisCode ac)
        {
            if(!_axisMapping.ContainsKey(ac))
                return 0.0f;

            return Input.GetAxis(_axisMapping[ac]);
        }

        public override Vector3 GetLookDirection()
        {
            return _camera.transform.forward;
        }

        public override Vector3 GetLeftAimDirection()
        {
            return _camera.transform.forward;
        }

        public override Vector3 GetRightAimDirection()
        {
            return _camera.transform.forward;
        }
    }

}