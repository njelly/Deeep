///////////////////////////////////////////////////////////////////////////////////////////////
//
//  AppManager (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/26/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using Tofunaut.Core;
using Tofunaut.SharpUnity;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeep
{
    // --------------------------------------------------------------------------------------------
    public class AppManager : MonoBehaviour
    {
        // --------------------------------------------------------------------------------------------
        public static class State
        {
            public const string Init = "init";
            public const string Loading = "loading";
            public const string Start = "start";
            public const string InGame = "in_game";
        }

        private static AppManager _instance;

        public static AssetManager AssetManager => _instance._assetManager;
        public static SharpCamera Camera => _instance._gameCamera;

        [Header("Debug")]
        [SerializeField] private bool _skipToGame;

        private TofuStateMachine _stateMachine;
        private AssetManager _assetManager;
        private SharpCamera _gameCamera;

        // --------------------------------------------------------------------------------------------
        private void Awake()
        {
            if(_instance != null)
            {
                Debug.LogError("AppManager instance is not null");
                Destroy(this);
                return;
            }

            _instance = this;

            _assetManager = new AssetManager();

            _stateMachine = new TofuStateMachine();
            _stateMachine.Register(State.Init, Init_Enter, null, null);
            _stateMachine.Register(State.Loading, null, null, null);
            _stateMachine.Register(State.Start, null, null, null);
            _stateMachine.Register(State.InGame, null, null, null);
            _stateMachine.ChangeState(State.Init);
        }

        // --------------------------------------------------------------------------------------------
        private void OnDestroy()
        {
            if(_instance == this)
            {
                _instance = null;
            }
        }

        // --------------------------------------------------------------------------------------------
        private void Update()
        {
            _stateMachine.Update(Time.deltaTime);
        }

        // --------------------------------------------------------------------------------------------
        private void Init_Enter()
        {
            _gameCamera = SharpCamera.Main();
            _gameCamera.Render(transform);

            _gameCamera.UnityCamera.orthographic = true;
            _gameCamera.LocalPosition = new Vector3(0f, 0f, -10f);

#if UNITY_EDITOR
            if(_skipToGame)
            {
                _stateMachine.ChangeState(State.InGame);
            }
#endif
        }
    }
}