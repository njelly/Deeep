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

        private const float TakeTurnCooldown = 1f;

        public class MoveModeChangedEvent : UnityEvent<EMoveMode, EMoveMode> { }

        public event EventHandler<MoveModeEventArgs> MoveModeChanged;
        public event EventHandler TakeTacticalTurn;

        public static PlayerActor Instance { get; private set; }
        public static EMoveMode MoveMode { get; private set; }

        public Holdable Holding { get; private set; }

        [Header("Player")]
        [SerializeField] private ActorReticle _reticle;

        private Transform _holdingPrevParent;
        private bool _playerHasTakenTacticalTurn;
        private TofuAnimation _tacticalTurnCooldownAnimation;

        // --------------------------------------------------------------------------------------------
        private void Awake()
        {
            if(Instance)
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
            if(MoveMode == EMoveMode.Tactical)
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
                        .Then()
                        .Execute(() =>
                        {
                            TakeTacticalTurn?.Invoke(this, EventArgs.Empty);
                            _playerHasTakenTacticalTurn = false;
                            _tacticalTurnCooldownAnimation = null;
                        })
                        .Play();
                }
            }

            base.Update();

            if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
            {
                if(MoveMode == EMoveMode.FreeMove)
                {
                    SetMoveMode(EMoveMode.Tactical);
                }
                else if(MoveMode == EMoveMode.Tactical)
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
                if(Holding.canHolderRotate)
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
        public void Hold(Holdable holdable)
        {
            if(Holding)
            {
                if(_holdingPrevParent)
                {
                    Holding.transform.SetParent(_holdingPrevParent);
                }
                Holding.transform.position = _targetPosition + _interactOffset;
            }

            Holding = holdable;

            if(Holding)
            {
                _holdingPrevParent = holdable.transform.parent;
                Holding.transform.SetParent(_reticle.transform);
            }
        }

        // --------------------------------------------------------------------------------------------
        private void SetMoveMode(EMoveMode moveMode)
        {
            if(moveMode == MoveMode)
            {
                return;
            }

            EMoveMode previousMode = MoveMode;
            MoveMode = moveMode;

            _playerHasTakenTacticalTurn = false;
            _tacticalTurnCooldownAnimation?.Stop();
            _tacticalTurnCooldownAnimation = null;

            MoveModeChanged?.Invoke(this, new MoveModeEventArgs(previousMode, MoveMode));
        }
    }

    // --------------------------------------------------------------------------------------------
    public class MoveModeEventArgs : EventArgs
    {
        public readonly PlayerActor.EMoveMode previousMode;
        public readonly PlayerActor.EMoveMode currentMode;

        public MoveModeEventArgs(PlayerActor.EMoveMode previousMode, PlayerActor.EMoveMode currentMode)
        {
            this.previousMode = previousMode;
            this.currentMode = currentMode;
        }
    }
}