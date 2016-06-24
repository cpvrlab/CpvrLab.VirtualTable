using UnityEngine;
using System.Collections.Generic;

namespace CpvrLab.VirtualTable
{

    // todo:    review the necessaty and function of this class
    //          it seems like the item equip handling could easily
    //          be done by concrete classes. Of course we need a base
    //          class to handle locking of input etc but maybe saving
    //          the input slots in here too is overkill and completely
    //          unnecessary. 
    //          Would it be better if we moved all the equip code into the
    //          UsableItem class?
    //
    public abstract class GamePlayer : MonoBehaviour
    {

        // defines an input slot that can serve an usable item with input
        protected class InputSlot
        {
            public PlayerInput input;
            public UsableItem item = null;
        }

        // prevent the player from equipping or unequipping any items
        public bool lockEquippedItems = false;

        // prevent the player from picking up any items
        public bool lockPickup = false;

        // prevent the player from using any equipped items
        public bool lockItemUse = false;


        protected List<InputSlot> _inputSlots = new List<InputSlot>();

        // todo:    use a unified naming scheme across all classes used for this project
        //          for the methods that can be used by subclasses. Maybe start isn't such a good idea
        //          we should maybe use an initialize method that gets called by start in the 
        protected virtual void Start()
        {
            // add ourselves to the current game manager
            GameManager.instance.AddPlayer(this);
        }
        
        protected InputSlot FindInputSlot(UsableItem item)
        {
            return _inputSlots.Find(x => x.item == item);
        }

        protected InputSlot FindInputSlot(PlayerInput input)
        {
            return _inputSlots.Find(x => x.input == input);
        }

        protected bool IsItemEquipped(UsableItem item)
        {
            return FindInputSlot(item) != null;
        }

        protected bool IsInputSlotAssigned(PlayerInput input)
        {
            return FindInputSlot(input) != null;
        }

        // equip an item and assign it to a specific player input
        public void Equip(PlayerInput input, UsableItem item, bool unequipIfOccupied = false)
        {
            var slot = FindInputSlot(input);
            if(slot == null) {
                Debug.LogWarning("GamePlayer: Trying to add an item to a non existant input slot");
                return;
            }
            
            // already an item equipped to that slot
            if(slot.item != null) {
                if(!unequipIfOccupied)
                    return;

                // unequip the current item
                Unequip(slot.item);
            }

            // assign new the item to the slot
            slot.item = item;

            // notify the concrete class and the item
            OnEquip(input, item);
            item.OnEquip(input);
        }

        // equip an item to the main input slot
        public void Equip(UsableItem item, bool unequipIfOccupied = false)
        {
            Equip(GetMainInput(), item, unequipIfOccupied);
        }

        public void Unequip(UsableItem item)
        {
            var slot = _inputSlots.Find(x => x.item == item);
            if(slot == null) {
                Debug.LogWarning("GamePlayer: Trying to unequip an item that wasn't equipped!");
                return;
            }

            slot.item = null;

            // notify the concrete class and the item
            OnUnequip(slot.input, item);
            item.OnUnequip();
        }

        // unequip an item assigned to a specific slot
        public void UnequipItemFrom(PlayerInput input)
        {
            var slot = FindInputSlot(input);
            if(slot.item == null || !IsItemEquipped(slot.item))
                return;

            Unequip(slot.item);
        }

        // unequips all equipped items
        public void UnequipAll()
        {
            foreach(var slot in _inputSlots) {
                if(slot.item != null) {
                    Unequip(slot.item);
                }
            }
        }

        // register a new input slot with the base class
        protected void AddInputSlot(PlayerInput input)
        {
            if(IsInputSlotAssigned(input)) {
                Debug.LogError("GamePlayer: Trying to add an player input that is already assigned to an input slot!");
                return;
            }

            var inputSlot = new InputSlot();
            inputSlot.input = input;

            _inputSlots.Add(inputSlot);
        }

        protected abstract void OnEquip(PlayerInput input, UsableItem item);
        protected abstract void OnUnequip(PlayerInput input, UsableItem item);
        protected abstract PlayerInput GetMainInput();
    }

}