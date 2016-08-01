using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.Characters.FirstPerson;

namespace CpvrLab.VirtualTable {


    /// <summary>
    /// Player using the oculus rift
    /// </summary>
    public class RiftPlayer : GamePlayer {

        [Header("Rift Player Properties")]
        public GameObject head;
        public GameObject cam;
        public GameObject leftHandGoal;
        public GameObject rightHandGoal;
        public GameObject localHands;
        public PlayerInput leftPlayerInput;
        public PlayerInput rightPlayerInput;

        public GameObject attachPointLeft;
        public GameObject attachPointRight;

        public override void OnStartClient()
        {
            base.OnStartClient();


            AddAttachmentSlot(attachPointLeft, leftPlayerInput);
            AddAttachmentSlot(attachPointRight, rightPlayerInput);
        }
        
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            cam.SetActive(true);
            localHands.SetActive(true);

            // enable hand goal scripts necessary for the local player to work
            leftHandGoal.GetComponent<HandPoseLerp>().enabled = true;
            leftHandGoal.GetComponent<HandConfidenceWeightFade>().enabled = true;
            rightHandGoal.GetComponent<HandPoseLerp>().enabled = true;
            rightHandGoal.GetComponent<HandConfidenceWeightFade>().enabled = true;

        }
        
        void Update()
        {
            if(!isLocalPlayer)
                return;

            head.transform.position = cam.transform.position;
            head.transform.rotation = cam.transform.rotation;
        }

        protected override PlayerInput GetMainInput()
        {
            return null;
        }

        protected override void OnEquip(AttachmentSlot slot)
        {
        }
        
        protected override void OnUnequip(UsableItem item)
        {
        }
        
    } // class
    
} // namespace