///////////////////////////////////////////////////////////////////////////////////////////////
//
//  DamageMatrix (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 03/01/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Tofunaut.UnityUtils;

namespace Tofunaut.Deeep.Game
{
    public class DamageMatrix : SingletonBehaviour<DamageMatrix>
    {
        // --------------------------------------------------------------------------------------------
        [Serializable]
        public struct DamagePairing
        {
            public ETargetType target;
            public EDamageType damage;
            public float amount;
        }

        protected override bool SetDontDestroyOnLoad => true;

        // TODO: create editor script to make this easier to modify
        public List<DamagePairing> damagePairings = new List<DamagePairing>();

        private float[][] _targetToDamageToType;

        // --------------------------------------------------------------------------------------------
        protected override void Awake()
        {
            base.Awake();

            // build the float array based on enum int values
            _targetToDamageToType = new float[Enum.GetValues(typeof(ETargetType)).Length][];
            for(int i = 0; i < damagePairings.Count; i++)
            {
                int targetTypeIndex = (int)damagePairings[i].target;
                int damageTypeIndex = (int)damagePairings[i].damage;
                if(_targetToDamageToType[targetTypeIndex] == null)
                {
                    _targetToDamageToType[targetTypeIndex] = new float[Enum.GetValues(typeof(EDamageType)).Length];
                }
                _targetToDamageToType[targetTypeIndex][damageTypeIndex] = damagePairings[i].amount;
            }
        }

        // --------------------------------------------------------------------------------------------
        public static float GetDamage(ETargetType targetType, EDamageType damageType)
        {
            return _instance._targetToDamageToType[(int)targetType][(int)damageType];
        }
    }

    // --------------------------------------------------------------------------------------------
    public enum ETargetType
    {
        Invalid,
        Player,
        Skeleton,
    }

    // --------------------------------------------------------------------------------------------
    public enum EDamageType
    {
        Invalid,
        Player_Melee,
        Sword_Melee,
        Skeleton_Melee,
    }
}