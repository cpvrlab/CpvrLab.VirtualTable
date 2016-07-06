using UnityEngine;
using System.Collections;
using System;

namespace CpvrLab.VirtualTable {


    /// <summary>
    /// LeapMotion PlayerInput. Not sure how we will do this yet, but we'll see.
    /// </summary>
    public class LeapPlayerInput : PlayerInput {
        public override bool GetAction(ActionCode ac)
        {
            throw new NotImplementedException();
        }

        public override bool GetActionDown(ActionCode ac)
        {
            throw new NotImplementedException();
        }

        public override bool GetActionUp(ActionCode ac)
        {
            throw new NotImplementedException();
        }

        public override float GetAxis(AxisCode ac)
        {
            throw new NotImplementedException();
        }

        public override Vector3 GetLeftAimDirection()
        {
            throw new NotImplementedException();
        }

        public override Vector3 GetLookDirection()
        {
            throw new NotImplementedException();
        }

        public override Vector3 GetRightAimDirection()
        {
            throw new NotImplementedException();
        }
    }

}