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
    /**
     * Manages the orientation of the bones in a model rigged for skeletal animation.
     * 
     * The class expects that the graphics model bones corresponding to bones in the Leap Motion 
     * hand model are in the same order in the bones array.
     */
    public class RiggedFingerOLD : FingerModel
    {

        /** Allows the mesh to be stretched to align with finger joint positions
         * Only set to true when mesh is not visible
         */
        public bool deformPosition = false;

        public Vector3 modelFingerPointing = Vector3.forward;
        public Vector3 modelPalmFacing = -Vector3.up;

        public Quaternion Reorientation()
        {
            return Quaternion.Inverse(Quaternion.LookRotation(modelFingerPointing, -modelPalmFacing));
        }

        /** Updates the bone rotations. */
        public override void UpdateFinger()
        {
            for (int i = 0; i < bones.Length; ++i)
            {
                if (bones[i] != null)
                {
                    bones[i].rotation = GetBoneRotation(i) * Reorientation();
                    if (deformPosition)
                    {
                        bones[i].position = GetBoneCenter(i);
                    }
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