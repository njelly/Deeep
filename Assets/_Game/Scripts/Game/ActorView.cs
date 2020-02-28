///////////////////////////////////////////////////////////////////////////////////////////////
//
//  ActorView (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/26/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class ActorView : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer _spriteRenderer;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected AnimatorOverrideController _overrideController;

        public GameObject SpriteRendererGameObject => _spriteRenderer.gameObject;

        protected Actor _actor;

        // --------------------------------------------------------------------------------------------
        protected virtual void Start()
        {
            if(_overrideController)
            {
                _animator.runtimeAnimatorController = _overrideController;
            }
        }

        // --------------------------------------------------------------------------------------------
        public static void Load(string prefabPath, Actor actor, Action<ActorView> callback)
        {
            AppManager.AssetManager.Load(prefabPath, (bool succesfull, GameObject payload) =>
            {
                if (succesfull)
                {
                    GameObject instantiatedPayload = Instantiate(payload, actor.Transform, false);
                    ActorView actorView = instantiatedPayload.GetComponent<ActorView>();
                    actorView._actor = actor;

                    callback(actorView);
                }
            });
        }
    }
}
