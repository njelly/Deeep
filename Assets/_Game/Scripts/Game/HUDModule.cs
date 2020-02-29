///////////////////////////////////////////////////////////////////////////////////////////////
//
//  HUDModule (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/27/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public abstract class HUDModule : MonoBehaviour
    {
        private static int _numBlockingInput;

        public static bool BlockPlayerInput => _numBlockingInput > 0;

        [Header("HUD Module")]
        [SerializeField] private bool _blockPlayerInput;

        // --------------------------------------------------------------------------------------------
        protected virtual void OnEnable()
        {
            if(_blockPlayerInput)
            {
                _numBlockingInput++;
            }
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void OnDisable()
        {
            if(_blockPlayerInput)
            {
                _numBlockingInput--;
            }
        }
    }
}