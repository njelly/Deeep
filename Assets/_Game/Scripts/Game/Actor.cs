///////////////////////////////////////////////////////////////////////////////////////////////
//
//  Actor (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/27/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public abstract class Actor : MonoBehaviour
    {
        public Vector3 TilePosition => new Vector3(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y));
        public ActorInput Input => _input;

        private const float MoveButtonHoldTimeThreshold = 0.08f;

        [Header("Components")]
        [SerializeField] protected SpriteRenderer _spriteRenderer;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected AnimatorOverrideController _overrideController;

        [Header("Actor")]
        [SerializeField] protected float _moveSpeed;

        protected ActorInput _input;
        protected Vector3 _targetPosition;
        protected Vector2 _interactOffset;

        // --------------------------------------------------------------------------------------------
        protected virtual void Start()
        {
            _input = new ActorInput();
            _targetPosition = transform.localPosition;

            if(_overrideController != null)
            {
                _animator.runtimeAnimatorController = _overrideController;
            }
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void Update()
        {
            UpdateInput();

            UpdateMovement();

            // try to interact
            if (_input.space.Pressed)
            {
                Interact();
            }
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void Interact() { }

        // --------------------------------------------------------------------------------------------
        protected virtual void UpdateMovement()
        {
            // Grid based movement!
            bool reachedDestination = transform.localPosition.IsApproximately(_targetPosition);
            Vector3 nextPosition = transform.localPosition;
            float stutterFix = 0f;
            if (!reachedDestination)
            {
                float moveDelta = _moveSpeed * Time.deltaTime;
                nextPosition = Vector3.MoveTowards(transform.localPosition, _targetPosition, moveDelta);

                reachedDestination = nextPosition.IsApproximately(_targetPosition);
                if (reachedDestination)
                {
                    Vector3 toNext = nextPosition - transform.localPosition;
                    stutterFix = moveDelta - toNext.magnitude;
                }
            }
            if (reachedDestination)
            {
                if (_input.up)
                {
                    _interactOffset = Vector3.up;
                    if (!_input.shift && _input.up.timeDown > MoveButtonHoldTimeThreshold)
                    {
                        _targetPosition += Vector3.up;
                    }
                }
                else if (_input.down)
                {
                    _interactOffset = Vector3.down;
                    if (!_input.shift && _input.down.timeDown > MoveButtonHoldTimeThreshold)
                    {
                        _targetPosition += Vector3.down;
                    }
                }
                else if (_input.left)
                {
                    _interactOffset = Vector3.left;
                    _spriteRenderer.flipX = true;
                    if (!_input.shift && _input.left.timeDown > MoveButtonHoldTimeThreshold)
                    {
                        _targetPosition += Vector3.left;
                    }
                }
                else if (_input.right)
                {
                    _interactOffset = Vector3.right;
                    _spriteRenderer.flipX = false;
                    if (!_input.shift && _input.right.timeDown > MoveButtonHoldTimeThreshold)
                    {
                        _targetPosition += Vector3.right;
                    }
                }

                nextPosition += (_targetPosition - nextPosition).normalized * stutterFix;
            }

            transform.localPosition = nextPosition;
        }

        // --------------------------------------------------------------------------------------------
        protected abstract void UpdateInput();
    }

    // --------------------------------------------------------------------------------------------
    public class ActorInput
    {
        public ActorInputButton up, down, left, right, shift, space, one, two, three, four;
    }

    // --------------------------------------------------------------------------------------------
    public struct ActorInputButton
    {
        public float timeDown;
        public bool wasDown;

        public bool Released => timeDown <= float.Epsilon && wasDown;
        public bool Held => timeDown > float.Epsilon && wasDown;
        public bool Pressed => timeDown > float.Epsilon && !wasDown;

        // --------------------------------------------------------------------------------------------
        public static implicit operator bool(ActorInputButton button)
        {
            return button.timeDown > float.Epsilon;
        }
    }
}