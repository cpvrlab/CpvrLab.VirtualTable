using UnityEngine;
using System.Collections;
using System;

namespace CpvrLab.VirtualTable {

    // todo:    how will we implement button presses from the leap?
    //          maybe a gui on the arm? problem here is we can't 
    //          press a button while picking something up
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