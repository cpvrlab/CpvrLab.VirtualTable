using UnityEngine;
using System.Collections;
using System;


namespace CpvrLab.VirtualTable {




    // todo: is it a good idea to make this a component? should it be?

    // todo:    would it be a good idea to use an other input propagation system?
    //          instead of querying the input class with GetActionDown etc could we
    //          send the events to subscribers? 
    public abstract class PlayerInput : MonoBehaviour {

        // todo:    Don't know how to call these at this point
        //          But this needs to be replaced by something more readable 
        //          ASAP!
        public enum ActionCode {
            Button0,
            Button1,
            Button2,
            Button3,
            Button4,
            Button5,
            Button6,
            Button7,
            Button8,
            Button9,

        }
        public enum AxisCode {
            Axis0,
            Axis1,
            Axis2,
            Axis3,
            Axis4,
            Axis5,
            Axis6,
            Axis7,
        }

        // is this action currently active
        public abstract bool GetAction(ActionCode ac);

        // was this action pressed during the current frame?
        public abstract bool GetActionDown(ActionCode ac);
        // was this action released during the current frame?
        public abstract bool GetActionUp(ActionCode ac);

        // get value of this axis
        public abstract float GetAxis(AxisCode ac);

        public abstract Vector3 GetLookDirection();
        public abstract Vector3 GetLeftAimDirection();
        public abstract Vector3 GetRightAimDirection();

    }

}