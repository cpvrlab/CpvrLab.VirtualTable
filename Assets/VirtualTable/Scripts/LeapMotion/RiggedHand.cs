/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Leap.Unity
{
    // Class to setup a rigged hand based on a model.
    public class RiggedHandOLD : HandModel
    {
        public override ModelType HandModelType
        {
            get
            {
                return ModelType.Graphics;
            }
        }


        public Vector3 modelFingerPointing = Vector3.forward;
        public Vector3 modelPalmFacing = -Vector3.up;

        public override void InitHand()
        {
            UpdateHand();
        }

        public Quaternion Reorientation()
        {
            return Quaternion.Inverse(Quaternion.LookRotation(modelFingerPointing, -modelPalmFacing));
        }

        public override void UpdateHand()
        {
            if (palm != null)
            {
                palm.position = GetPalmPosition();
                palm.rotation = GetPalmRotation() * Reorientation();
            }

            if (forearm != null)
                forearm.rotation = GetArmRotation() * Reorientation();

            for (int i = 0; i < fingers.Length; ++i)
            {
                if (fingers[i] != null)
                {
                    fingers[i].fingerType = (Finger.FingerType)i;
                    fingers[i].UpdateFinger();
                }
            }
        }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.color = Color.white;
        Handles.ArrowCap(0, transform.position, transform.rotation * Reorientation(), 0.05f);

        Quaternion fingerPointing = Quaternion.FromToRotation(Vector3.forward, modelFingerPointing);
        Quaternion palmFacing = Quaternion.FromToRotation(Vector3.forward, modelPalmFacing);

        Handles.color = new Color(1.0f, 0.4f, 0.0f);
        Handles.ArrowCap(0, transform.position, transform.rotation * fingerPointing, 0.05f);

        Handles.color = new Color(0.0f, 0.7f, 1.0f);
        Handles.ArrowCap(0, transform.position, transform.rotation * palmFacing, 0.05f);
    }
#endif
    }

}