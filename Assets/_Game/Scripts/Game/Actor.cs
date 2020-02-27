///////////////////////////////////////////////////////////////////////////////////////////////
//
//  Actor (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/27/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using TofuCore;
using Tofunaut.SharpUnity;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public abstract class Actor : SharpGameObject, Updater.IUpdateable
    {
        public IntVector2 Coord => new IntVector2(Mathf.RoundToInt(Transform.localPosition.x), Mathf.RoundToInt(Transform.localPosition.y));

        private readonly string _prefabPath;

        protected ActorView _actorView;
        protected ActorInput _input;
        protected Vector2 _targetPosition;

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

            AppManager.AssetManager.Load(_prefabPath, (bool succesfull, GameObject payload) =>
            {
                if(succesfull)
                {
                    GameObject instantiatedPayload = Object.Instantiate(payload, Transform, false);
                    _actorView = instantiatedPayload.GetComponent<ActorView>();
                    if(_actorView == null)
                    {
                        Debug.Log($"the prefab at path {_prefabPath} has no ActorView component");
                    }
                }
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
        public void Update(float deltaTime)
        {
            UpdateInput();

            if(_input.left.JustPressed)
            {
                _actorView.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            if (_input.right.JustPressed)
            {
                _actorView.transform.localScale = new Vector3(1f, 1f, 1f);
            }

        }

        // --------------------------------------------------------------------------------------------
        protected abstract void UpdateInput();
    }

    // --------------------------------------------------------------------------------------------
    public class ActorInput
    {
        public ActorInputButton up, down, left, right, shift, one, two, three, four;
    }

    // --------------------------------------------------------------------------------------------
    public struct ActorInputButton
    {
        public bool isDown;
        public bool wasDown;

        public bool Released => !isDown && wasDown;
        public bool Held => isDown && wasDown;
        public bool JustPressed => isDown && !wasDown;

        // --------------------------------------------------------------------------------------------
        public static implicit operator bool(ActorInputButton button)
        {
            return button.isDown;
        }
    }
}