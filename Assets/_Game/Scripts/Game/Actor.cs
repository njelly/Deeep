///////////////////////////////////////////////////////////////////////////////////////////////
//
//  Actor (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/27/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using System;
using TofuCore;
using Tofunaut.SharpUnity;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public abstract class Actor : SharpGameObject, Updater.IUpdateable
    {
        public event EventHandler OnInteract;

        public Vector3 TilePosition => new Vector3(Mathf.RoundToInt(Transform.localPosition.x), Mathf.RoundToInt(Transform.localPosition.y));
        public ActorInput Input => _input;
        public Vector3 InteractOffset { get; private set; }


        private const float MoveButtonHoldTimeThreshold = 0.08f;

        private readonly string _prefabPath;

        protected ActorView _actorView;
        protected ActorInput _input;
        protected Vector3 _targetPosition;

        // --------------------------------------------------------------------------------------------
        protected Actor(string name, string prefabPath) : base(name)
        {
            _prefabPath = prefabPath;
            _input = new ActorInput();
        }

        // --------------------------------------------------------------------------------------------
        protected override void Build()
        {
            Updater.Instance.Add(this);

            _targetPosition = Transform.position;

            ActorView.Load(_prefabPath, this, (ActorView actorView) =>
            {
                _actorView = actorView;
            });
        }

        // --------------------------------------------------------------------------------------------
        public override void Destroy()
        {
            base.Destroy();

            if (Updater.HasInstance)
            {
                Updater.Instance.Remove(this);
            }
        }

        // --------------------------------------------------------------------------------------------
        public virtual void Update(float deltaTime)
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
        protected virtual void Interact()
        {
            OnInteract?.Invoke(this, EventArgs.Empty);
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void UpdateMovement()
        {
            // Grid based movement!
            bool reachedDestination = Transform.localPosition.IsApproximately(_targetPosition);
            Vector3 nextPosition = Transform.localPosition;
            float stutterFix = 0f;
            if (!reachedDestination)
            {
                float moveDelta = 5f * Time.deltaTime;
                nextPosition = Vector3.MoveTowards(Transform.localPosition, _targetPosition, moveDelta);

                reachedDestination = nextPosition.IsApproximately(_targetPosition);
                if (reachedDestination)
                {
                    Vector3 toNext = nextPosition - Transform.localPosition;
                    stutterFix = moveDelta - toNext.magnitude;
                }
            }
            if (reachedDestination)
            {
                if (_input.up)
                {
                    InteractOffset = Vector3.up;
                    if (!_input.shift && _input.up.timeDown > MoveButtonHoldTimeThreshold)
                    {
                        _targetPosition += Vector3.up;
                    }
                }
                else if (_input.down)
                {
                    InteractOffset = Vector3.down;
                    if (!_input.shift && _input.down.timeDown > MoveButtonHoldTimeThreshold)
                    {
                        _targetPosition += Vector3.down;
                    }
                }
                else if (_input.left)
                {
                    InteractOffset = Vector3.left;
                    _actorView.SpriteRendererGameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
                    if (!_input.shift && _input.left.timeDown > MoveButtonHoldTimeThreshold)
                    {
                        _targetPosition += Vector3.left;
                    }
                }
                else if (_input.right)
                {
                    InteractOffset = Vector3.right;
                    _actorView.SpriteRendererGameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                    if (!_input.shift && _input.right.timeDown > MoveButtonHoldTimeThreshold)
                    {
                        _targetPosition += Vector3.right;
                    }
                }

                nextPosition += (_targetPosition - nextPosition).normalized * stutterFix;
            }

            Transform.localPosition = nextPosition;
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