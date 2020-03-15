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
    public class InventoryItem : MonoBehaviour
    {
        [HideInInspector, NonSerialized] public Inventory inventory;
        [SerializeField] protected SpriteRenderer _spriteRenderer;

        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        [Header("Item")]
        public float weight;
        public float goldValue;

        // --------------------------------------------------------------------------------------------
        private void Start()
        {
            Interactable interactable = GetComponent<Interactable>();
            if (interactable)
            {
                interactable.AddBeginInteractListener(OnBeginInteract);
            }
        }

        // --------------------------------------------------------------------------------------------
        private void OnDestroy()
        {
            Interactable interactable = GetComponent<Interactable>();
            if (interactable)
            {
                interactable.RemoveBeginInteractListener(OnBeginInteract);
            }
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void OnBeginInteract(Interactable.InteractedEventInfo info)
        {
            if (inventory)
            {
                inventory.Remove(this, false);
            }

            inventory = info.instigator.Inventory;

            if (inventory)
            {
                inventory.Add(this);
            }
        }
    }
}