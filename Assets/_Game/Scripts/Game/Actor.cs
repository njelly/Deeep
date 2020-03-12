///////////////////////////////////////////////////////////////////////////////////////////////
//
//  Actor (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/27/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Tofunaut.Animation;
using Tofunaut.UnityUtils;
using UnityEngine;
using UnityEngine.Events;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public enum EAlliance
    {
        None,
        Player,
        Enemy,
    }

    // --------------------------------------------------------------------------------------------
    public enum EFacing
    {
        None,
        Left,
        Right,
    }

    // --------------------------------------------------------------------------------------------
    public abstract class Actor : MonoBehaviour, ActorInput.IReceiver
    {
        [Serializable]
        public struct InteractedEventInfo
        {
            public Interactable interactedWith;
        }
        protected class InteractedEvent : UnityEvent<InteractedEventInfo> { }

        private const float MoveButtonHoldTimeThreshold = 0.08f;

        [Header("Components")]
        [SerializeField] protected SpriteRenderer _spriteRenderer;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected Collider2D _collider;

        [Header("Actor")]
        [SerializeField] protected Inventory _inventory;
        [SerializeField] protected Destructible _destructible;
        [SerializeField] protected Weapon _equippedWeapon;
        [SerializeField] protected float _moveSpeed;
        [SerializeField] protected EAlliance _alliance;

        [Space(10)]
        [SerializeField] protected InteractedEvent _onInteracted = new InteractedEvent();

        public Inventory Inventory => _inventory;
        public Vector3 TilePosition => new Vector3(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y));
        public ActorInput Input => _input;
        public Vector3 InteractOffset => _interactOffset;
        public EAlliance Alliance => _alliance;
        public Weapon EquipedWeapon => _equippedWeapon;
        public EFacing Facing => _facing;

        protected ActorInput _input;
        protected Vector3 _targetPosition;
        protected Vector3 _previousPosition;
        protected Vector3 _interactOffset;
        protected EFacing _facing;

        public void AddInteractedListener(UnityAction<InteractedEventInfo> action) => _onInteracted.AddListener(action);
        public void RemoveInteractedListener(UnityAction<InteractedEventInfo> action) => _onInteracted.RemoveListener(action);

        // --------------------------------------------------------------------------------------------
        protected virtual void Start()
        {
            _input = new ActorInput();
            _targetPosition = transform.localPosition;
            _interactOffset = Vector3.right;
            _facing = EFacing.Right;

            if (_destructible)
            {
                _destructible.AddDamageListener(OnDamaged);
            }
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void OnDestroy()
        {
            if (_destructible)
            {
                _destructible.RemoveDamageListener(OnDamaged);
            }
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void Update()
        {
            if (PlayerActor.MoveMode == PlayerActor.EMoveMode.FreeMove)
            {
                TryChooseNextTargetPosition();
                TryMoveInteractOffset();
                TryInteract();
            }

            UpdateMovement();
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void UpdateMovement()
        {
            _previousPosition = transform.localPosition;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, _targetPosition, _moveSpeed * Time.deltaTime);
            _collider.offset = _targetPosition - transform.position;
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void TryChooseNextTargetPosition()
        {
            if (_input.shift)
            {
                // the Shift key makes sure the actor stays in place
                return;
            }

            if (!_targetPosition.IsApproximately(transform.localPosition))
            {
                // wait until we've reached our current target before choosing the next target
                return;
            }

            // poll our inputs
            Vector3 potentialTargetPos = _targetPosition;
            if (_input.up.timeDown > MoveButtonHoldTimeThreshold)
            {
                potentialTargetPos += Vector3.up;
            }
            else if (_input.down.timeDown > MoveButtonHoldTimeThreshold)
            {
                potentialTargetPos += Vector3.down;
            }
            else if (_input.left.timeDown > MoveButtonHoldTimeThreshold)
            {
                potentialTargetPos += Vector3.left;
            }
            else if (_input.right.timeDown > MoveButtonHoldTimeThreshold)
            {
                potentialTargetPos += Vector3.right;
            }

            if (!CanOccupyPosition(potentialTargetPos))
            {
                // we can't move there, return
                return;
            }

            if (!_previousPosition.IsApproximately(transform.localPosition))
            {
                // compensate for stutter if we are continuously moving
                float amountWouldHaveMoved = _moveSpeed * Time.deltaTime - (_previousPosition - transform.localPosition).magnitude;
                Vector3 stutterFix = (potentialTargetPos - transform.localPosition).normalized * amountWouldHaveMoved;
                transform.localPosition += stutterFix;
            }

            _targetPosition = potentialTargetPos;
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void TryMoveInteractOffset()
        {
            if (_input.up && CanTurnInteractOffset(Vector3.up))
            {
                _interactOffset = Vector3.up;
            }
            else if (_input.down && CanTurnInteractOffset(Vector3.down))
            {
                _interactOffset = Vector3.down;
            }
            else if (_input.left && CanTurnInteractOffset(Vector3.left))
            {
                _interactOffset = Vector3.left;
                _spriteRenderer.flipX = true;
                _facing = EFacing.Left;
            }
            else if (_input.right && CanTurnInteractOffset(Vector3.right))
            {
                _interactOffset = Vector3.right;
                _spriteRenderer.flipX = false;
                _facing = EFacing.Right;
            }
        }

        // --------------------------------------------------------------------------------------------
        private delegate void InteractDelegate(Interactable interactable);
        protected virtual void TryInteract()
        {
            InteractDelegate interactDelegate = null;
            if (_input.space.Pressed)
            {
                interactDelegate = BeginInteract;
            }
            else if (_input.space.Released)
            {
                interactDelegate = EndInteract;
            }

            Collider2D[] facingColliders = Physics2D.OverlapCircleAll(_targetPosition + _interactOffset, 0.4f);
            if (interactDelegate != null)
            {
                foreach (Collider2D collider in facingColliders)
                {
                    Interactable[] interactables = collider.GetComponents<Interactable>();
                    foreach (Interactable interactable in interactables)
                    {
                        interactDelegate(interactable);
                    }
                }
            }
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void BeginInteract(Interactable interactable)
        {
            interactable.BeginInteract(this);
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void EndInteract(Interactable interactable)
        {
            interactable.EndInteract(this);
        }

        // --------------------------------------------------------------------------------------------
        protected virtual bool CanOccupyPosition(Vector3 position)
        {
            return Physics2D.OverlapCircleAll(position.Vector2_XY(), 0.4f, LayerMask.GetMask("Blocking", "Actor")).Length == 0;
        }

        // --------------------------------------------------------------------------------------------
        protected virtual bool CanTurnInteractOffset(Vector3 potentialInteractOffset)
        {
            return true;
        }

        // --------------------------------------------------------------------------------------------
        public void ReceivePlayerInput(ActorInput input)
        {
            _input = input;
        }

        // --------------------------------------------------------------------------------------------
        public void OnDamaged(Destructible.DamageEventInfo damageEventInfo)
        {
            if (damageEventInfo.DidKill)
            {
                Quaternion startRot = Quaternion.Euler(0f, 0f, 0f);
                Quaternion endRot = Quaternion.Euler(90f, 0f, 0f);

                // TODO: temp death anim
                new TofuAnimation()
                    .Value01(1f, EEaseType.EaseInOutBack, (float newValue) =>
                    {
                        if(this) // check that we aren't destroyed
                        {
                            transform.localRotation = Quaternion.LerpUnclamped(startRot, endRot, newValue);
                        }
                    })
                    .Then()
                    .Execute(() =>
                    {
                        if(this)
                        {
                            Destroy(gameObject);
                        }
                    })
                    .Play();
            }
        }

        // --------------------------------------------------------------------------------------------
        public bool IsAlliedWith(Actor other)
        {
            if (other._alliance == EAlliance.None || _alliance == EAlliance.None)
            {
                return false;
            }
            else
            {
                return other._alliance == _alliance;
            }
        }

        // --------------------------------------------------------------------------------------------
        public void EquipWeapon(Weapon weapon)
        {
            if(_equippedWeapon)
            {
                _equippedWeapon.OnUnequipped();
                _inventory.Add(_equippedWeapon);
            }

            weapon.owner = this;
            weapon.OnEquipped();
            weapon.gameObject.SetActive(true);
            weapon.transform.SetParent(transform, false);
            weapon.transform.localPosition = Vector3.zero;
            _equippedWeapon = weapon;
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