using UnityEngine;
using System.Collections;

namespace CpvrLab.VirtualTable {

    public class PrototypeGun : EquippableItem {
        

        void Update()
        {
            if(_input == null)
                return;

            if(_input.GetActionDown(PlayerInput.ActionCode.Button0))
                Debug.Log("boom");
        }
    }

}