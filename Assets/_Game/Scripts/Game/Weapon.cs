///////////////////////////////////////////////////////////////////////////////////////////////
//
//  Weapon (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 03/11/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public abstract class Weapon : InventoryItem
    {
        [Header("Weapon")]
        public Actor owner;
        [SerializeField] protected EDamageType _damageType;

        public EDamageType DamageType => _damageType;

        // --------------------------------------------------------------------------------------------
        public virtual void OnEquipped() 
        {
            owner.AddInteractedListener(OnOwnerInteracted);
        }

        // --------------------------------------------------------------------------------------------
        public virtual void OnUnequipped()
        {
            owner.RemoveInteractedListener(OnOwnerInteracted);
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
        private void OnOwnerInteracted(Interactable interactable) 
        {
            if(!owner.EquipedWeapon == this)
            {
                return;
            }

            if(!(interactable is Destructible))
            {
                return;
            }

            // TODO: trigger some animation here?
        }
    }
}