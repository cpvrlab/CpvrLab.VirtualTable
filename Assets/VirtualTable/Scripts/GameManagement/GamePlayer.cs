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
            GameManager.instance.AddPlayer(this);
        }

        // equip an item to a specific input slot
        protected void EquipItem(PlayerInput input, UsableItem item)
        {
            var slot = _inputSlots.Find(x => x.input == input);
            if (slot == null)
            {
                Debug.LogError("GamePlayer: Trying to add an item to a non existant input slot");
                return;
            }

            // don't allow the user to equip an item to a slot with already an item present
            if (slot.item != null)
            {
                Debug.LogError("GamePlayer: Trying to equip an item to a slot with already an item equipped!");
                return;
            }
            

            slot.item = item;

            // notify the item
            item.OnEquip(input);
        }

        protected void UnequipItem(UsableItem item)
        {
            var slot = _inputSlots.Find(x => x.item == item);
            if (slot == null)
            {
                Debug.LogError("GamePlayer: Trying to unequip an item that wasn't equipped!");
                return;
            }

            slot.item = null;
            
            // notify the item
            item.OnUnequip();
        }

        protected UsableItem GetItemInSlot(PlayerInput input)
        {
            var slot = _inputSlots.Find(x => x.input == input);
            return slot.item;
        }

        protected virtual void OnEquipItem(UsableItem item) { }
        protected virtual void OnUnequipItem(UsableItem item) { }


        // register a new input slot with the base class
        protected void AddInputSlot(PlayerInput input)
        {
            var inputSlot = new InputSlot();
            inputSlot.input = input;

            _inputSlots.Add(inputSlot);
        }
    }

}