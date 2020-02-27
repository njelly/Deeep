///////////////////////////////////////////////////////////////////////////////////////////////
//
//  ActorView (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/26/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class ActorView : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer _spriteRenderer;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected AnimatorOverrideController _overrideController;

        // --------------------------------------------------------------------------------------------
        protected virtual void Start()
        {
            if(_overrideController)
            {
                _animator.runtimeAnimatorController = _overrideController;
            }
        }
    }
}
