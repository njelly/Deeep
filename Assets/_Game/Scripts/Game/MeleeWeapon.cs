///////////////////////////////////////////////////////////////////////////////////////////////
//
//  MeleeWeapon (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 03/11/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class MeleeWeapon : Weapon
    {
        // --------------------------------------------------------------------------------------------
        public override bool CanAttackDestructible(Destructible destructible)
        {
            if(!owner)
            {
                return false;
            }

            if(Vector2.Distance(owner.transform.position, destructible.transform.position) > 1.1f)
            {
                return false;
            }

            return base.CanAttackDestructible(destructible);
        }
    }
}