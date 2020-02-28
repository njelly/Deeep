///////////////////////////////////////////////////////////////////////////////////////////////
//
//  PlayerActorView (c) 2020 Tofunaut
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
    public class PlayerReticle : MonoBehaviour
    {
        private const float ReticleMoveAnimTime = 0.1f;
        private const float ReticleFlashAnimTime = 0.4f;

        [SerializeField] private SpriteRenderer _spriteRenderer;

        public Color CurrentColor => _currentReticleColor;

        private TofuAnimation _reticleMoveAnimation;
        private TofuAnimation _reticleFlashAnimation;
        private Color _defaultReticleColor;
        private Color _currentReticleColor;

        private Vector3 _previousInteractOffset;

        // --------------------------------------------------------------------------------------------
        private void Start()
        {
            _defaultReticleColor = _spriteRenderer.color;
            _currentReticleColor = _defaultReticleColor;
        }

        // --------------------------------------------------------------------------------------------
        public void AnimateInteractReticleMove(Vector3 interactOffset)
        {
            if(_previousInteractOffset.IsApproximately(interactOffset))
            {
                return;
            }

            _previousInteractOffset = interactOffset;

            _reticleMoveAnimation?.Stop();

            float fromAngle = Mathf.Atan2(_spriteRenderer.transform.localPosition.x, -_spriteRenderer.transform.localPosition.y) - Mathf.PI / 2f;
            float toAngle = Mathf.Atan2(interactOffset.x, -interactOffset.y) - Mathf.PI / 2f;

            if (toAngle - fromAngle > Mathf.PI)
            {
                toAngle -= Mathf.PI * 2f;
            }
            if (fromAngle - toAngle > Mathf.PI)
            {
                fromAngle -= Mathf.PI * 2f;
            }

            _reticleMoveAnimation = new TofuAnimation()
                .Value01(ReticleMoveAnimTime, EEaseType.Linear, (float newValue) =>
                {
                    float angle = Mathf.LerpUnclamped(fromAngle, toAngle, newValue);
                    _spriteRenderer.transform.localPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                })
                .Play();
        }

        // --------------------------------------------------------------------------------------------
        public void AnimateInteractReticleColor()
        {
            _spriteRenderer.color = _currentReticleColor;
            AnimateInteractReticleColor(1f);
        }
        public void AnimateInteractReticleColor(float alpha) => AnimateInteractReticleColor(new Color(_currentReticleColor.r, _currentReticleColor.g, _currentReticleColor.b, alpha));
        public void AnimateInteractReticleColor(Color flashColor)
        {
            _currentReticleColor = new Color(flashColor.r, flashColor.g, flashColor.b, _defaultReticleColor.a);

            _reticleFlashAnimation?.Stop();
            _reticleFlashAnimation = new TofuAnimation()
                .Value01(ReticleFlashAnimTime, EEaseType.EaseOutExpo, (float newValue) =>
                {
                    _spriteRenderer.color = Color.LerpUnclamped(_currentReticleColor, flashColor, newValue);
                })
                .Then()
                .Execute(() =>
                {
                    _reticleFlashAnimation = null;
                })
                .Play();
        }
    }
}
