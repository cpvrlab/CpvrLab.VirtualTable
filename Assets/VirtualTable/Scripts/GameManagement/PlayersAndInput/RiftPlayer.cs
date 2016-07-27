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

        public override void OnStartClient()
        {
            base.OnStartClient();
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            cam.SetActive(true);
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