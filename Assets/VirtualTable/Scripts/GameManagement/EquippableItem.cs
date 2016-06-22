using UnityEngine;
using System.Collections;

namespace CpvrLab.VirtualTable {
    public class EquippableItem : MonoBehaviour {

        public Transform attachPoint;
        protected PlayerInput _input;

        public void Equip(PlayerInput input)
        {
            _input = input;
        }

        public void Unequip()
        {
            _input = null;
        }
    }
}