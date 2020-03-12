///////////////////////////////////////////////////////////////////////////////////////////////
//
//  MeleeWeapon (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 03/11/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using Tofunaut.Animation;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class MeleeWeapon : Weapon
    {
        private const float LungeAnimDistance = 0.3f;
        private Vector3 _preAttackPos;

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

        protected override TofuAnimation PreAttackAnimation()
        {
            Vector3 startPos = owner.transform.position;
            Vector3 lungePos = startPos + (CurrentlyAttacking.transform.position - owner.transform.position).normalized * LungeAnimDistance;

            _preAttackPos = startPos;

            return new TofuAnimation()
                .Value01(_preAttackDelay, EEaseType.EaseInExpo, (float newValue) =>
                {
                    owner.transform.position = Vector3.LerpUnclamped(startPos, lungePos, newValue);
                });
        }

        protected override void DoAttack()
        {
            CurrentlyAttacking.TakeDamage(_damageType);
        }

        protected override TofuAnimation PostAttackAnimation()
        {
            Vector3 startPos = owner.transform.position;
            Vector3 endPos = _preAttackPos;

            return new TofuAnimation()
                .Value01(_postAttackCooldown, EEaseType.Linear, (float newValue) =>
                {
                    owner.transform.position = Vector3.LerpUnclamped(startPos, endPos, newValue);
                })
                .Then()
                .Execute(() =>
                {
                    owner.transform.position = owner.transform.position.RoundToInt();
                });
        }
    }
}