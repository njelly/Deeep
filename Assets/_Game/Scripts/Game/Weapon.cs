///////////////////////////////////////////////////////////////////////////////////////////////
//
//  Weapon (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 03/11/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using Tofunaut.UnityUtils;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public abstract class Weapon : InventoryItem
    {
        [Header("Weapon")]
        public Actor owner;
        [SerializeField] protected EDamageType _damageType;

        public EDamageType DamageType => _damageType;

        private void OnEnable()
        {
            if(!owner)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            }
        }

        // --------------------------------------------------------------------------------------------
        private void Update()
        {
            if(owner)
            {
                switch(owner.Facing)
                {
                    case EFacing.Left:
                        _spriteRenderer.flipX = true;
                        break;
                    case EFacing.Right:
                        _spriteRenderer.flipX = false;
                        break;
                }
            }
        }

        // --------------------------------------------------------------------------------------------
        public virtual void OnEquipped() 
        {
            owner.AddInteractedListener(OnOwnerInteracted);

            transform.rotation = Quaternion.identity;

            _spriteRenderer.sortingLayerName = "Default";
            _spriteRenderer.sortingOrder = 1;
        }

        // --------------------------------------------------------------------------------------------
        public virtual void OnUnequipped()
        {
            owner.RemoveInteractedListener(OnOwnerInteracted);

            _spriteRenderer.sortingLayerName = "Floor";
            _spriteRenderer.sortingOrder = 0;
        }

        // --------------------------------------------------------------------------------------------
        public override void BeginInteract(Actor instigator)
        {
            if (inventory)
            {
                inventory.Remove(this, false);
            }

            inventory = instigator.Inventory;

            if (inventory)
            {
                inventory.Add(this, true, () =>
                {
                    if(!instigator.EquipedWeapon)
                    {
                        inventory.Remove(this, false);
                        instigator.EquipWeapon(this);
                    }
                });
            }
        }

        // --------------------------------------------------------------------------------------------
        public virtual bool CanAttackDestructible(Destructible destructible)
        {
            if(!owner)
            {
                return false;
            }

            Actor destructibleActor = destructible.gameObject.GetComponentInParent<Actor>();
            if(destructibleActor)
            {
                return !owner.IsAlliedWith(destructibleActor);
            }

            return true;
        }

        // --------------------------------------------------------------------------------------------
        private void OnOwnerInteracted(Actor.InteractedEventInfo info) 
        {
            if(!owner.EquipedWeapon == this)
            {
                return;
            }

            if(!(info.interactedWith is Destructible))
            {
                return;
            }

            // TODO: trigger some animation here?
        }
    }
}