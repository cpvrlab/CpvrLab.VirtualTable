using UnityEngine;
using System.Collections;

namespace CpvrLab.VirtualTable {
    public class EquippableItem : MonoBehaviour {

        public Transform attachPoint;
        protected PlayerInput _input;
        protected Transform _parent;
        
        public void Equip(PlayerInput input)
        {
            // save previous parent
            // todo:    we should handle the whole pickup stuff
            //          somewhere central, either here or in the GamePlayer base class
            _parent = transform.parent;

            _input = input;
            OnEquip();
        }

        public void Unequip()
        {
            transform.parent = _parent;

            _input = null;
            OnUnequip();
        }

        protected virtual void OnEquip() { }
        protected virtual void OnUnequip() { }
    }
}