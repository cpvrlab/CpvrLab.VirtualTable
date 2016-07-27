﻿using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CpvrLab.AVRtar {
    /// <summary>
    /// @todo rename
    /// maps the hand pose from the aVRtar hand setup to an arbitrary other hand setup
    /// </summary>
    public class HandPoseMapper : MonoBehaviour {

        public Transform palm;
        public Vector3 fingerForward = Vector3.forward;
        public Vector3 palmDirection = Vector3.down;

        public FingerPoseMapper[] fingers = new FingerPoseMapper[5];

        public HandMapping otherHand;

        public bool invertPalm = false;


        public void CalculateAxes()
        {
            // @todo auto detect palm forward and palm direction and call the same function for all the connected fingers

            //1. estimate palm direction
            Vector3 AB = fingers[1].transform.position - palm.position;
            Vector3 AC = fingers[4].transform.position - palm.position;

            palmDirection = Vector3.Cross(AB, AC).normalized;
            palmDirection = Quaternion.Inverse(palm.rotation) * palmDirection;
            if(invertPalm) palmDirection *= -1.0f;

            Vector3 fingersAvrgPosition = Vector3.zero;
            int fingerAvrgCount = 0;

            for(int i = 0; i < fingers.Length; ++i) {
                if(!fingers[i])
                    continue;

                fingers[i].palmDirection = palmDirection;
                fingers[i].CalculateAxes();

                // we don't include the thumb in the average because it wont line up with the palm most of the time
                if(i > 0) {
                    fingersAvrgPosition += fingers[i].transform.position;
                    fingerAvrgCount++;
                }
            }

            fingersAvrgPosition /= fingerAvrgCount;

            fingerForward = (fingersAvrgPosition - palm.position).normalized;
            fingerForward = Quaternion.Inverse(palm.rotation) * fingerForward;

            // SceneView.RepaintAll();
        }

        public Quaternion Reorientation()
        {
            return Quaternion.Inverse(Quaternion.LookRotation(fingerForward, -palmDirection));
        }

        public void UpdatePose()
        {
            if(otherHand == null)
                return;

            if(palm != null) {
                palm.position = otherHand.palm.position;
                palm.rotation = otherHand.palm.rotation * Reorientation();
            }

            for(int i = 0; i < fingers.Length; ++i) {
                if(fingers[i] != null) {
                    fingers[i].UpdatePose(otherHand.GetFingerBones((HandMapping.FingerType)i));
                }
            }
        }

        void LateUpdate()
        {
            UpdatePose();
        }
#if UNITY_EDITOR

        void OnDrawGizmos()
        {
            Handles.color = Color.white;
            Handles.ArrowCap(0, transform.position, transform.rotation * Reorientation(), 0.05f);

            Quaternion fingerPointing = Quaternion.FromToRotation(Vector3.forward, fingerForward);
            Quaternion palmFacing = Quaternion.FromToRotation(Vector3.forward, palmDirection);

            Handles.color = new Color(1.0f, 0.4f, 0.0f);
            Handles.ArrowCap(0, transform.position, transform.rotation * fingerPointing, 0.05f);

            Handles.color = new Color(0.0f, 0.7f, 1.0f);
            Handles.ArrowCap(0, transform.position, transform.rotation * palmFacing, 0.05f);
        }
#endif
    }
}