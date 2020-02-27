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
            public const string StartMenu = "start_menu";
            public const string InGame = "in_game";
        }

        private static AppManager _instance;

        public static AssetManager AssetManager => _instance._assetManager;
        public static SharpCamera Camera => _instance._camera;

        [Header("Debug")]
        [SerializeField] private bool _skipToGame;

        private TofuStateMachine _stateMachine;
        private AssetManager _assetManager;
        private SharpCamera _camera;

        // --------------------------------------------------------------------------------------------
        private void Awake()
        {
            if(_instance != null)
            {
                Debug.LogError("AppManager already exists");
                Destroy(this);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            _assetManager = new AssetManager();

            _stateMachine = new TofuStateMachine();
            _stateMachine.Register(State.Init, Init_Enter, Init_Update, null);
            _stateMachine.Register(State.StartMenu, StartMenu_Enter, null, null);
            _stateMachine.Register(State.InGame, InGame_Enter, null, InGame_Exit);
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

        #region State Machine

        // --------------------------------------------------------------------------------------------
        private void Init_Enter()
        {
            _camera = SharpCamera.Main();
            _camera.CameraOrthographic = true;
            _camera.LocalPosition = new Vector3(0f, 0f, -10f);
            _camera.Render(transform);

            AssetManager.Load<TMPro.TMP_FontAsset>(AssetPaths.Fonts.Code);
        }

        // --------------------------------------------------------------------------------------------
        private void Init_Update(float deltaTime)
        {
            if(_assetManager.Ready)
            {
#if UNITY_EDITOR
                if (_skipToGame)
                {
                    _stateMachine.ChangeState(State.InGame);
                    return;
                }
#endif
                _stateMachine.ChangeState(State.StartMenu);
            }
        }

        // --------------------------------------------------------------------------------------------
        private void StartMenu_Enter()
        {
            Debug.Log("start menu");
        }

        // --------------------------------------------------------------------------------------------
        private void InGame_Enter()
        {
            InGameController inGameController = gameObject.RequireComponent<InGameController>();
            inGameController.enabled = true;
            inGameController.Completed += InGameController_Completed;
        }

        // --------------------------------------------------------------------------------------------
        private void InGame_Exit()
        {
            InGameController inGameController = gameObject.RequireComponent<InGameController>();
            inGameController.enabled = false;
            inGameController.Completed -= InGameController_Completed;
        }

        #endregion State Machine

        // --------------------------------------------------------------------------------------------
        private void InGameController_Completed(object sender, ControllerCompletedEventArgs e)
        {
            InGameControllerCompletedEventArgs inGameControllerCompletedEventArgs = e as InGameControllerCompletedEventArgs;
            switch(inGameControllerCompletedEventArgs.intention)
            {
                case InGameControllerCompletedEventArgs.Intention.StartMenu:
                    _stateMachine.ChangeState(State.StartMenu);
                    break;
                case InGameControllerCompletedEventArgs.Intention.QuitApp:
                    QuitApp();
                    break;
            }
        }

        // --------------------------------------------------------------------------------------------
        private void QuitApp()
        {
            // TODO: do some app clean up here
            Application.Quit();
        }
    }
}