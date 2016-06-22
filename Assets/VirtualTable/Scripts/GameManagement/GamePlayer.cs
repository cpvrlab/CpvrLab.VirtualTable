﻿using UnityEngine;
using System.Collections.Generic;

namespace CpvrLab.VirtualTable {

    public abstract class GamePlayer : MonoBehaviour {

        // prevent the player from equipping or unequipping any items
        public bool lockEquippedItems = false;

        // prevent the player from picking up any items
        public bool lockPickup = false;

        // prevent the player from using any equipped items
        public bool lockItemUse = false;

        protected List<EquippableItem> _equippedItems = new List<EquippableItem>();

        protected PlayerInput _playerInput;
        
        // todo:    use a unified naming scheme across all classes used for this project
        //          for the methods that can be used by subclasses. Maybe start isn't such a good idea
        //          we should maybe use an initialize method that gets called by start in the 
        protected virtual void Start()
        {
            GameManager.instance.AddPlayer(this);
            _playerInput = GetComponent<PlayerInput>();

            if(_playerInput == null) {
                Debug.LogError("GamePlayer: Couldn't find a PlayerInput component attached to the GamePlayer GameObject!");
            }
        }

        protected void EquipItem(EquippableItem ei)
        {
            // todo: sanity checks
            _equippedItems.Add(ei);
            ei.Equip(_playerInput);
        }

        protected void UnequipItem(EquippableItem ei)
        {
            // todo: sanity checks
            _equippedItems.Remove(ei);
            ei.Unequip();
        }
    }

}