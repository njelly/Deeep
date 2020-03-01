///////////////////////////////////////////////////////////////////////////////////////////////
//
//  InventoryItem (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/29/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class InventoryItem : Interactable
    {
        [HideInInspector, NonSerialized] public Inventory inventory;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        [Header("Item")]
        public float weight;
        public float goldValue;

        // --------------------------------------------------------------------------------------------
        public override void BeginInteract(Actor instigator)
        {
            if(inventory)
            {
                inventory.Remove(this, false);
            }

            inventory = instigator.Inventory;

            if(inventory)
            {
                inventory.Add(this);
            }
        }

        // --------------------------------------------------------------------------------------------
        public override void EndInteract(Actor instigator) { }
    }
}