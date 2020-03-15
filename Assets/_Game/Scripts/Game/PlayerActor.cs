///////////////////////////////////////////////////////////////////////////////////////////////
//
//  PlayerActor (c) 2020 Tofunaut
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
    public class PlayerActor : Actor
    {
        // --------------------------------------------------------------------------------------------
        public enum EMoveMode
        {
            FreeMove,
            Tactical,
            Paused,
        }

        // --------------------------------------------------------------------------------------------
        public struct MoveModeChangedInfo
        {
            public EMoveMode previousMode;
            public EMoveMode currentMode;
        }

        [Serializable]
        public class MoveModeChangedEvent : UnityEvent<MoveModeChangedInfo> { }

        private const float TakeTurnCooldown = 1f;

        public static PlayerActor Instance { get; private set; }
        public static EMoveMode MoveMode { get; private set; }

        [Space(10)]
        [SerializeField] private MoveModeChangedEvent _moveModeChanged;
        [SerializeField] private UnityEvent _takeTacticalTurn;

        private bool _playerHasTakenTacticalTurn;
        private TofuAnimation _tacticalTurnCooldownAnimation;

        public void AddTakeTacticalTurnListener(UnityAction action) => _takeTacticalTurn.AddListener(action);
        public void RemoveTakeTacticalTurnListener(UnityAction action) => _takeTacticalTurn.RemoveListener(action);
        public void AddMoveModeChangedListener(UnityAction<MoveModeChangedInfo> action) => _moveModeChanged.AddListener(action);
        public void RemoveMoveModeChangedListener(UnityAction<MoveModeChangedInfo> action) => _moveModeChanged.RemoveListener(action);

        // --------------------------------------------------------------------------------------------
        private void Awake()
        {
            if (Instance)
            {
                Debug.LogError("Only one PlayerActor can exist at a time!");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            MoveMode = EMoveMode.FreeMove;
        }

        // --------------------------------------------------------------------------------------------
        private void OnEnable()
        {
            PlayerInputManager.Add(this);
        }

        // --------------------------------------------------------------------------------------------
        private void OnDisable()
        {
            PlayerInputManager.Remove(this);
        }

        // --------------------------------------------------------------------------------------------
        protected override void Update()
        {
            if (MoveMode == EMoveMode.Tactical)
            {
                if (!_playerHasTakenTacticalTurn)
                {
                    TryChooseNextTargetPosition();
                }

                TryMoveInteractOffset();

                if (!_playerHasTakenTacticalTurn)
                {
                    TryInteract();
                }

                if (_playerHasTakenTacticalTurn && _tacticalTurnCooldownAnimation == null)
                {
                    _tacticalTurnCooldownAnimation = new TofuAnimation()
                        .Wait(TakeTurnCooldown)
                        .WaitUntil(() =>
                        {
                            // wait for the player's attack to complete before telling all the other actors to take their turn 
                            return !(EquipedWeapon && EquipedWeapon.IsAttacking);
                        })
                        .Then()
                        .Execute(() =>
                        {
                            _takeTacticalTurn?.Invoke();
                            _playerHasTakenTacticalTurn = false;
                            _tacticalTurnCooldownAnimation = null;
                        })
                        .Play();
                }
            }

            base.Update();

            if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
            {
                if (MoveMode == EMoveMode.FreeMove)
                {
                    SetMoveMode(EMoveMode.Tactical);
                }
                else if (MoveMode == EMoveMode.Tactical)
                {
                    SetMoveMode(EMoveMode.FreeMove);
                }
            }
        }

        // --------------------------------------------------------------------------------------------
        protected override void TryChooseNextTargetPosition()
        {
            switch (MoveMode)
            {
                case EMoveMode.FreeMove:
                    base.TryChooseNextTargetPosition();
                    break;
                case EMoveMode.Tactical:
                    if (!_playerHasTakenTacticalTurn)
                    {
                        Vector3 previousTargetPosition = _targetPosition;
                        base.TryChooseNextTargetPosition();
                        _playerHasTakenTacticalTurn = !previousTargetPosition.IsApproximately(_targetPosition);
                    }
                    break;
            }
        }


        // --------------------------------------------------------------------------------------------
        protected override void TryInteract()
        {
            if (MoveMode == EMoveMode.Tactical && _tacticalTurnCooldownAnimation != null)
            {
                return;
            }

            base.TryInteract();
        }

        // --------------------------------------------------------------------------------------------
        protected override void BeginInteract(Interactable interactable)
        {
            _playerHasTakenTacticalTurn = true;
            base.BeginInteract(interactable);
        }

        // --------------------------------------------------------------------------------------------
        protected override void EndInteract(Interactable interactable)
        {
            _playerHasTakenTacticalTurn = true;
            base.EndInteract(interactable);
        }

        // --------------------------------------------------------------------------------------------
        protected override bool CanOccupyPosition(Vector3 position)
        {
            if (MoveMode == EMoveMode.Tactical && _tacticalTurnCooldownAnimation != null)
            {
                return false;
            }

            if (Holding)
            {
                return Physics2D.OverlapCircleAll(position + _interactOffset, 0.4f, LayerMask.GetMask("Blocking", "Actor")).Length == 0 && base.CanOccupyPosition(position);
            }
            else
            {
                return base.CanOccupyPosition(position);
            }
        }

        // --------------------------------------------------------------------------------------------
        protected override bool CanTurnInteractOffset(Vector3 potentialInteractOffset)
        {
            if (Holding)
            {
                if (Holding.canHolderRotate)
                {
                    bool doesCollide = false;
                    foreach (Collider2D collider in Physics2D.OverlapCircleAll(_targetPosition + potentialInteractOffset, 0.4f))
                    {
                        doesCollide |= !Physics.GetIgnoreLayerCollision(collider.gameObject.layer, Holding.gameObject.layer);
                    }
                    return !doesCollide;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return base.CanTurnInteractOffset(potentialInteractOffset);
            }
        }

        // --------------------------------------------------------------------------------------------
        private void SetMoveMode(EMoveMode moveMode)
        {
            if (moveMode == MoveMode)
            {
                return;
            }

            EMoveMode previousMode = MoveMode;
            MoveMode = moveMode;

            _playerHasTakenTacticalTurn = false;
            _tacticalTurnCooldownAnimation?.Stop();
            _tacticalTurnCooldownAnimation = null;

            _moveModeChanged?.Invoke(new MoveModeChangedInfo
            {
                previousMode = previousMode,
                currentMode = MoveMode,
            });
        }
    }
}