///////////////////////////////////////////////////////////////////////////////////////////////
//
//  HUDMoveMode (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 03/01/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using Tofunaut.Animation;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class HUDMoveMode : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _moveModeLabel;
        [SerializeField] private float _cameraFlashTime = 0.5f;
        [SerializeField] private Color _cameraFlashColor = Color.white;

        private TofuAnimation _cameraBGColorAnim;

        // --------------------------------------------------------------------------------------------
        private void Start()
        {
            PlayerActor.Instance.MoveModeChanged += PlayerActor_MoveModeChanged;
            SetMoveModeText(PlayerActor.MoveMode);
        }

        // --------------------------------------------------------------------------------------------
        private void SetMoveModeText(PlayerActor.EMoveMode moveMode) => SetMoveModeText(moveMode, false);
        private void SetMoveModeText(PlayerActor.EMoveMode moveMode, bool flashCamera)
        {
            switch(moveMode)
            {
                case PlayerActor.EMoveMode.FreeMove:
                    _moveModeLabel.text = "Free Move";
                    break;
                case PlayerActor.EMoveMode.Tactical:
                    _moveModeLabel.text = "Tactical";
                    break;
                case PlayerActor.EMoveMode.Paused:
                    _moveModeLabel.text = "Paused";
                    break;
            }

            if(flashCamera)
            {
                _cameraBGColorAnim?.Stop();
                _cameraBGColorAnim = new TofuAnimation()
                    .Value01(_cameraFlashTime / 2f, EEaseType.EaseOutExpo, (float newValue) =>
                    {
                        Camera.main.backgroundColor = Color.LerpUnclamped(Color.black, _cameraFlashColor, newValue);
                    })
                    .Then()
                    .Value01(_cameraFlashTime / 2f, EEaseType.EaseOutExpo, (float newValue) =>
                    {
                        Camera.main.backgroundColor = Color.LerpUnclamped(_cameraFlashColor, Color.black, newValue);
                    })
                    .Then()
                    .Execute(() =>
                    {
                        _cameraBGColorAnim = null;
                    })
                    .Play();
            }
        }

        // --------------------------------------------------------------------------------------------
        private void PlayerActor_MoveModeChanged(object sender, MoveModeEventArgs e)
        {
            SetMoveModeText(e.currentMode, true);
        }
    }
}