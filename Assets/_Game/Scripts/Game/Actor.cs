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
        private const float MoveButtonHoldTimeThreshold = 0.08f;

        [Header("Components")]
        [SerializeField] protected SpriteRenderer _spriteRenderer;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected AnimatorOverrideController _overrideController;
        [SerializeField] protected Collider2D _collider;

        [Header("Actor")]
        [SerializeField] protected Inventory _inventory;
        [SerializeField] protected float _moveSpeed;

        public Inventory Inventory => _inventory;
        public Vector3 TilePosition => new Vector3(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y));
        public ActorInput Input => _input;

        protected ActorInput _input;
        protected Vector3 _targetPosition;
        protected Vector3 _interactOffset;

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
            UpdateMovement();
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void LateUpdate()
        {
            _collider.offset = _targetPosition - transform.position;
        }

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
                Vector3 previousTarget = _targetPosition;
                if (_input.up)
                {
                    if(CanTurnInteractOffset(Vector3.up))
                    {
                        _interactOffset = Vector3.up;
                    }
                    if (!_input.shift && _input.up.timeDown > MoveButtonHoldTimeThreshold)
                    {
                        _targetPosition += Vector3.up;
                    }
                }
                else if (_input.down)
                {
                    if (CanTurnInteractOffset(Vector3.down))
                    {
                        _interactOffset = Vector3.down;
                    }
                    if (!_input.shift && _input.down.timeDown > MoveButtonHoldTimeThreshold)
                    {
                        _targetPosition += Vector3.down;
                    }
                }
                else if (_input.left)
                {
                    if (CanTurnInteractOffset(Vector3.left))
                    {
                        _interactOffset = Vector3.left;
                        _spriteRenderer.flipX = true;
                    }
                    if (!_input.shift && _input.left.timeDown > MoveButtonHoldTimeThreshold)
                    {
                        _targetPosition += Vector3.left;
                    }
                }
                else if (_input.right)
                {
                    if (CanTurnInteractOffset(Vector3.right))
                    {
                        _interactOffset = Vector3.right;
                        _spriteRenderer.flipX = false;
                    }
                    if (!_input.shift && _input.right.timeDown > MoveButtonHoldTimeThreshold)
                    {
                        _targetPosition += Vector3.right;
                    }
                }

                if(!_targetPosition.IsApproximately(previousTarget) && CanMoveToTargetPosition())
                {
                    nextPosition += (_targetPosition - nextPosition).normalized * stutterFix;
                }
                else
                {
                    _targetPosition = previousTarget;
                }
            }

            transform.localPosition = nextPosition;
        }

        // --------------------------------------------------------------------------------------------
        protected virtual bool CanMoveToTargetPosition()
        {
            return Physics2D.OverlapCircleAll(_targetPosition.Vector2_XY(), 0.4f, LayerMask.GetMask("Blocking", "Actor")).Length == 0;
        }

        // --------------------------------------------------------------------------------------------
        protected virtual bool CanTurnInteractOffset(Vector3 potentialInteractOffset)
        {
            return true;
        }
    }

    // --------------------------------------------------------------------------------------------
    public class ActorInput
    {
        public interface IReceiver
        {
            void ReceivePlayerInput(ActorInput input);
        }

        public ActorInputButton up, down, left, right, shift, space, one, two, three, four;

        // --------------------------------------------------------------------------------------------
        public static ActorInput PollPlayerInput(ActorInput input)
        {
            input.up.wasDown = input.up;
            input.up.timeDown = (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) ? input.up.timeDown + Time.deltaTime : 0;
            input.down.wasDown = input.down;
            input.down.timeDown = (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) ? input.down.timeDown + Time.deltaTime : 0;
            input.left.wasDown = input.left;
            input.left.timeDown = (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) ? input.left.timeDown + Time.deltaTime : 0;
            input.right.wasDown = input.right;
            input.right.timeDown = (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) ? input.right.timeDown + Time.deltaTime : 0;
            input.shift.wasDown = input.shift;
            input.shift.timeDown = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? input.shift.timeDown + Time.deltaTime : 0;
            input.space.wasDown = input.space;
            input.space.timeDown = Input.GetKey(KeyCode.Space) ? input.space.timeDown + Time.deltaTime : 0;
            input.one.wasDown = input.one;
            input.one.timeDown = Input.GetKey(KeyCode.Alpha1) ? input.one.timeDown + Time.deltaTime : 0;
            input.two.wasDown = input.two;
            input.two.timeDown = Input.GetKey(KeyCode.Alpha2) ? input.two.timeDown + Time.deltaTime : 0;
            input.three.wasDown = input.three;
            input.three.timeDown = Input.GetKey(KeyCode.Alpha3) ? input.three.timeDown + Time.deltaTime : 0;
            input.four.wasDown = input.four;
            input.four.timeDown = Input.GetKey(KeyCode.Alpha4) ? input.four.timeDown + Time.deltaTime : 0;

            return input;
        }
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