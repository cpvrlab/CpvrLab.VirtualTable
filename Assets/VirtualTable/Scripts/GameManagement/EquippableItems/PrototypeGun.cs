using UnityEngine;
using System.Collections;

namespace CpvrLab.VirtualTable {

    public class PrototypeGun : EquippableItem {
        

        void Update()
        {
            if(_input == null)
                return;

            // todo: this seems tedious, can't we just subscribe to buttons and receive input?
            if(_input.GetActionDown(PlayerInput.ActionCode.Button0))
                Debug.Log("boom");


        }
    }

}