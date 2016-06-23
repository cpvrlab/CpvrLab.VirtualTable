using UnityEngine;
using System.Collections.Generic;

namespace CpvrLab.VirtualTable {

    public abstract class GamePlayer : MonoBehaviour {

        // defines an input slot that can serve an equippable item with input
        protected class InputSlot {
            public PlayerInput input;
            public EquippableItem item;
        }

        // prevent the player from equipping or unequipping any items
        public bool lockEquippedItems = false;

        // prevent the player from picking up any items
        public bool lockPickup = false;

        // prevent the player from using any equipped items
        public bool lockItemUse = false;

        protected List<EquippableItem> _equippedItems = new List<EquippableItem>();

        protected List<InputSlot> _inputSlots = new List<InputSlot>();
                
        // todo:    use a unified naming scheme across all classes used for this project
        //          for the methods that can be used by subclasses. Maybe start isn't such a good idea
        //          we should maybe use an initialize method that gets called by start in the 
        protected virtual void Start()
        {
            GameManager.instance.AddPlayer(this);
        }
        
        protected void EquipItem(PlayerInput input, EquippableItem item)
        {
            //_inputSlots.
        }
        protected void UnequipItem(PlayerInput input, EquippableItem item)
        {

        }

        protected void AddInputSlot(PlayerInput input)
        {
            var inputSlot = new InputSlot();
            inputSlot.input = input;

            _inputSlots.Add(inputSlot);
        }
    }

}