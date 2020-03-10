///////////////////////////////////////////////////////////////////////////////////////////////
//
//  UIActorHealthBar (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 03/07/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using Tofunaut.Animation;
using UnityEngine;
using UnityEngine.UI;

namespace Tofunaut.Deeep.Game
{
    public class UIActorHealthBar : MonoBehaviour
    {
        [Header("Actor")]
        [SerializeField] private Destructible _destructible;

        [Header("UI")]
        [SerializeField] private Slider _slider;
        [SerializeField] private float _sliderAnimTime;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeAnimTime;

        private TofuAnimation _sliderAnimation;
        private TofuAnimation _fadeAnimation;
        private bool _visible;

        // --------------------------------------------------------------------------------------------
        private void Start()
        {
            _slider.value = _destructible.HealthPercent;
            _canvasGroup.alpha = 0f;

            if(PlayerActor.Instance)
            {
                PlayerActor.Instance.AddMoveModeChangedListener(OnMoveModeChanged);
            }

            if(_destructible)
            {
                _destructible.AddDamageListener(OnDamaged);
            }
        }

        // --------------------------------------------------------------------------------------------
        private void OnDestroy()
        {
            if (PlayerActor.Instance)
            {
                PlayerActor.Instance.RemoveMoveModeChangedListener(OnMoveModeChanged);
            }

            if (_destructible)
            {
                _destructible.RemoveDamageListener(OnDamaged);
            }
        }

        // --------------------------------------------------------------------------------------------
        public void OnMoveModeChanged(PlayerActor.MoveModeChangedInfo info)
        {
            switch(info.currentMode)
            {
                case PlayerActor.EMoveMode.Tactical:
                    if(!_visible)
                    {
                        SetVisible(true);
                    }
                    break;
                case PlayerActor.EMoveMode.FreeMove:
                    if(_sliderAnimation == null)
                    {
                        SetVisible(false);
                    }
                    break;
            }
        }

        // --------------------------------------------------------------------------------------------
        private void OnDamaged(Destructible.DamageEventInfo info)
        {
            if(!_visible)
            {
                SetVisible(true);
            }

            float startAmount = _slider.value;
            float endAmount = info.currentHealth / Destructible.MaxHealth;

            _sliderAnimation?.Stop();
            _sliderAnimation = new TofuAnimation()
                .Value01(_sliderAnimTime, EEaseType.Linear, (float newValue) =>
                {
                    _slider.value = Mathf.LerpUnclamped(startAmount, endAmount, newValue);
                })
                .WaitUntil(() =>
                {
                    return _fadeAnimation == null;
                })
                .Then()
                .Execute(() =>
                {
                    if(PlayerActor.MoveMode == PlayerActor.EMoveMode.FreeMove)
                    {
                        SetVisible(false);
                    }
                })
                .Then()
                .Execute(() =>
                {
                    _sliderAnimation = null;
                })
                .Play();
        }

        // --------------------------------------------------------------------------------------------
        private void SetVisible(bool visible)
        {
            _visible = visible;

            _fadeAnimation?.Stop();

            float startAlpha = _canvasGroup.alpha;
            float endAlpha = visible ? 1f : 0f;
            float time = visible ? _fadeAnimTime * (1f - startAlpha) : _fadeAnimTime * startAlpha;

            _fadeAnimation = new TofuAnimation()
                .Value01(time, EEaseType.Linear, (float newValue) =>
                {
                    _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, newValue);
                })
                .Then()
                .Execute(() =>
                {
                    _fadeAnimation = null;
                })
                .Play();
        }

    }
}
