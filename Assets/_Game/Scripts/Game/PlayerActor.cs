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

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class PlayerActor : Actor, ActorInput.IReceiver
    {
        // --------------------------------------------------------------------------------------------
        public enum EMoveMode
        {
            FreeMove,
            Tactical,
            Paused,
        }

        private const float TakeTurnCooldown = 1f;

        public event EventHandler<MoveModeEventArgs> MoveModeChanged;
        public event EventHandler TakeTacticalTurn;

        public static PlayerActor Instance { get; private set; }
        public static EMoveMode MoveMode { get; private set; }

        [Header("Player")]
        [SerializeField] protected PlayerReticle _playerReticle;

        public Holdable Holding { get; private set; }

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
            base.Update();

            _playerReticle.AnimateInteractReticleMove(_interactOffset);

            if (_input.space.Released)
            {
                // turn off the player reticle flash, this comes before the tactical check
                _playerReticle.AnimateInteractReticleColor(_playerReticle.CurrentColor);
            }

            if (MoveMode == EMoveMode.Tactical && _playerHasTakenTacticalTurn)
            {
                if(_tacticalTurnCooldownAnimation == null)
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

                return;
            }

            if (_input.space.Pressed)
            {
                // turn on the player reticle flash
                _playerReticle.AnimateInteractReticleColor();
            }

            _playerHasTakenTacticalTurn |= !transform.localPosition.IsApproximately(_targetPosition);

            // try to interact
            if (_input.space.Pressed)
            {
                BeginInteract();
            }
            else if (_input.space.Released)
            {
                EndInteract();
            }

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
        protected override bool CanMoveToTargetPosition()
        {
            if(MoveMode == EMoveMode.Tactical && _tacticalTurnCooldownAnimation != null)
            {
                return false;
            }

            if (Holding)
            {
                return Physics2D.OverlapCircleAll(_targetPosition + _interactOffset, 0.4f, LayerMask.GetMask("Blocking", "Actor")).Length == 0 && base.CanMoveToTargetPosition();
            }
            else
            {
                return base.CanMoveToTargetPosition();
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
        protected virtual void BeginInteract()
        {
            bool didInteract = false;
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(_targetPosition + _interactOffset, 0.4f))
            {
                foreach (Interactable interactable in collider.GetComponents<Interactable>())
                {
                    interactable.BeginInteract(this);
                    didInteract |= true;
                }
            }

            _playerHasTakenTacticalTurn |= didInteract;
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void EndInteract()
        {
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(_targetPosition + _interactOffset, 0.4f))
            {
                foreach (Interactable interactable in collider.GetComponents<Interactable>())
                {
                    interactable.EndInteract(this);
                }
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
                Holding.transform.SetParent(_playerReticle.transform);
            }
        }

        // --------------------------------------------------------------------------------------------
        public void ReceivePlayerInput(ActorInput input)
        {
            _input = input;
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