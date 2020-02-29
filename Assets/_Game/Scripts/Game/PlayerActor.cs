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

            if (_input.space.Pressed)
            {
                _playerReticle.AnimateInteractReticleColor();
            }
            else if (_input.space.Released)
            {
                _playerReticle.AnimateInteractReticleColor(_playerReticle.CurrentColor);
            }

            _playerReticle.AnimateInteractReticleMove(_interactOffset);
        }

        // --------------------------------------------------------------------------------------------
        protected override void UpdateInput()
        {
            _input.up.wasDown = _input.up;
            _input.up.timeDown = (UnityEngine.Input.GetKey(KeyCode.UpArrow) || UnityEngine.Input.GetKey(KeyCode.W))? _input.up.timeDown + Time.deltaTime : 0;
            _input.down.wasDown = _input.down;
            _input.down.timeDown = (UnityEngine.Input.GetKey(KeyCode.DownArrow) || UnityEngine.Input.GetKey(KeyCode.S)) ? _input.down.timeDown + Time.deltaTime : 0;
            _input.left.wasDown = _input.left;
            _input.left.timeDown = (UnityEngine.Input.GetKey(KeyCode.LeftArrow) || UnityEngine.Input.GetKey(KeyCode.A)) ? _input.left.timeDown + Time.deltaTime : 0;
            _input.right.wasDown = _input.right;
            _input.right.timeDown = (UnityEngine.Input.GetKey(KeyCode.RightArrow) || UnityEngine.Input.GetKey(KeyCode.D)) ? _input.right.timeDown + Time.deltaTime : 0;
            _input.shift.wasDown = _input.shift;
            _input.shift.timeDown = (UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift)) ? _input.shift.timeDown + Time.deltaTime : 0;
            _input.space.wasDown = _input.space;
            _input.space.timeDown = UnityEngine.Input.GetKey(KeyCode.Space) ? _input.space.timeDown + Time.deltaTime : 0;
            _input.one.wasDown = _input.one;
            _input.one.timeDown = UnityEngine.Input.GetKey(KeyCode.Alpha1) ? _input.one.timeDown + Time.deltaTime : 0;
            _input.two.wasDown = _input.two;
            _input.two.timeDown = UnityEngine.Input.GetKey(KeyCode.Alpha2) ? _input.two.timeDown + Time.deltaTime : 0;
            _input.three.wasDown = _input.three;
            _input.three.timeDown = UnityEngine.Input.GetKey(KeyCode.Alpha3) ? _input.three.timeDown + Time.deltaTime : 0;
            _input.four.wasDown = _input.four;
            _input.four.timeDown = UnityEngine.Input.GetKey(KeyCode.Alpha4) ? _input.four.timeDown + Time.deltaTime : 0;
        }

        // --------------------------------------------------------------------------------------------
        protected override bool CanMoveToTargetPosition()
        {
            if(Holding)
            {
                return Physics2D.OverlapCircleAll(_targetPosition + _interactOffset, 0.4f, LayerMask.GetMask("Blocking", "Actor")).Length == 0;
            }
            else
            {
                return base.CanMoveToTargetPosition();
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