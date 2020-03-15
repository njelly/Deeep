///////////////////////////////////////////////////////////////////////////////////////////////
//
//  Destructible (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 03/01/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class Destructible : MonoBehaviour
    {
        public const float MaxHealth = 100f; // all destructibles start at max health

        // --------------------------------------------------------------------------------------------
        [Serializable]
        public struct DamageEventInfo
        {
            public bool DidKill => currentHealth <= 0;

            public EDamageType damageType;
            public ETargetType targetType;
            public float previousHealth;
            public float currentHealth;
        }

        // --------------------------------------------------------------------------------------------
        [Serializable]
        public class DamageEvent : UnityEvent<DamageEventInfo> { }

        [SerializeField] private ETargetType _targetType;

        [Space(10)]
        [SerializeField] private DamageEvent _onDamaged;

        public ETargetType TargetType => _targetType;
        public float HealthPercent => _health / MaxHealth;

        public void AddDamageListener(UnityAction<DamageEventInfo> action) => _onDamaged.AddListener(action);
        public void RemoveDamageListener(UnityAction<DamageEventInfo> action) => _onDamaged.RemoveListener(action);

        private float _health;

        // --------------------------------------------------------------------------------------------
        private void Awake()
        {
            _health = MaxHealth;
        }

        // --------------------------------------------------------------------------------------------
        public void TakeDamage(EDamageType damageType)
        {
            float previousHealth = _health;
            _health -= DamageMatrix.GetDamage(_targetType, damageType);

            _onDamaged?.Invoke(new DamageEventInfo
            {
                damageType = damageType,
                targetType = TargetType,
                previousHealth = previousHealth,
                currentHealth = _health,
            });
        }
    }
}