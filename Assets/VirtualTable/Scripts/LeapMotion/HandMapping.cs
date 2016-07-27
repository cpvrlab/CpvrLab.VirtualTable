﻿using UnityEngine;
using System.Collections;
using Leap;

namespace CpvrLab.AVRtar {

    public class HandMapping : MonoBehaviour {

        public enum FingerType {
            Thumb,
            Index,
            Middle,
            Ring,
            Little
        }

        public Transform palm;
        public Transform[] thumb = new Transform[4];
        public Transform[] index = new Transform[4];
        public Transform[] middle = new Transform[4];
        public Transform[] ring = new Transform[4];
        public Transform[] little = new Transform[4];

        public Transform[] GetFingerBones(FingerType fingerType)
        {
            switch(fingerType) {
                case FingerType.Thumb: return thumb;
                case FingerType.Index: return index;
                case FingerType.Middle: return middle;
                case FingerType.Ring: return ring;
                case FingerType.Little: return little;
            }

            return null;
        }

        public Transform GetThumbBone(Bone.BoneType boneType)
        {
            return thumb[(int)boneType];
        }
        public Transform GetIndexBone(Bone.BoneType boneType)
        {
            return thumb[(int)boneType];
        }
        public Transform GetMiddleBone(Bone.BoneType boneType)
        {
            return thumb[(int)boneType];
        }
        public Transform GetRingBone(Bone.BoneType boneType)
        {
            return thumb[(int)boneType];
        }
        public Transform GetLittleBone(Bone.BoneType boneType)
        {
            return thumb[(int)boneType];
        }
    }
}