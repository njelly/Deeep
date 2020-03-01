///////////////////////////////////////////////////////////////////////////////////////////////
//
//  Destructible (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 03/01/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class Destructible : MonoBehaviour
    {
        public const float MaxHealth = 100f; // all destructibles start at max health

        [SerializeField] private ETargetType _targetType;

        public ETargetType TargetType => _targetType;

        private float _health;

        // --------------------------------------------------------------------------------------------
        private void Awake()
        {
            _health = MaxHealth;
        }

        // --------------------------------------------------------------------------------------------
        public void TakeDamage(EDamageType damageType)
        {
            _health -= DamageMatrix.GetDamage(_targetType, damageType);
            if(_health <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}