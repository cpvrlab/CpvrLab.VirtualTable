using UnityEngine;
using System.Collections;
using System;

namespace CpvrLab.VirtualTable {

    public class LeapPlayer : GamePlayer {
        protected override PlayerInput GetMainInput()
        {
            throw new NotImplementedException();
        }

        protected override void OnEquip(PlayerInput input, UsableItem item)
        {
            throw new NotImplementedException();
        }

        protected override void OnUnequip(UsableItem item)
        {
            throw new NotImplementedException();
        }
    }

}