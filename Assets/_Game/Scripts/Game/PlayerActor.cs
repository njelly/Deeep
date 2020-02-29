///////////////////////////////////////////////////////////////////////////////////////////////
//
//  PlayerActor (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/27/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class PlayerActor : Actor
    {
        [Header("Player")]
        [SerializeField] protected PlayerReticle _playerReticle;

        public Holdable Holding { get; private set; }

        private Transform _holdingPrevParent;

        // --------------------------------------------------------------------------------------------
        protected override void Update()
        {
            base.Update();

            if(!HUDModule.BlockPlayerInput)
            {
                if (_input.space.Pressed)
                {
                    _playerReticle.AnimateInteractReticleColor();
                }
                else if (_input.space.Released)
                {
                    _playerReticle.AnimateInteractReticleColor(_playerReticle.CurrentColor);
                }

                _playerReticle.AnimateInteractReticleMove(_interactOffset);

                // try to interact
                if (_input.space.Pressed)
                {
                    BeginInteract();
                }
                else if (_input.space.Released)
                {
                    EndInteract();
                }
            }
        }

        // --------------------------------------------------------------------------------------------
        protected override void UpdateInput()
        {
            if(!HUDModule.BlockPlayerInput)
            {
                _input = ActorInput.PollPlayerInput(_input);
            }
        }

        // --------------------------------------------------------------------------------------------
        protected override bool CanMoveToTargetPosition()
        {
            if(Holding)
            {
                return Physics2D.OverlapCircleAll(_targetPosition + _interactOffset, 0.4f, LayerMask.GetMask("Blocking", "Actor")).Length == 0 && base.CanMoveToTargetPosition();
            }
            else
            {
                return base.CanMoveToTargetPosition();
            }
        }

        protected override bool CanTurnInteractOffset(Vector3 potentialInteractOffset)
        {
            if(Holding)
            {
                bool doesCollide = false;
                foreach(Collider2D collider in Physics2D.OverlapCircleAll(_targetPosition + potentialInteractOffset, 0.4f))
                {
                    doesCollide |= !Physics.GetIgnoreLayerCollision(collider.gameObject.layer, Holding.gameObject.layer);
                }
                return !doesCollide;
            }
            else
            {
                return base.CanTurnInteractOffset(potentialInteractOffset);
            }
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void BeginInteract()
        {
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(_targetPosition + _interactOffset, 0.4f))
            {
                foreach (Interactable interactable in collider.GetComponents<Interactable>())
                {
                    interactable.BeginInteract(this);
                }
            }
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
    }
}