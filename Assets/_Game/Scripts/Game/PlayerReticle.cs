///////////////////////////////////////////////////////////////////////////////////////////////
//
//  PlayerReticle (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/27/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using Tofunaut.Animation;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class PlayerReticle : MonoBehaviour
    {
        private const float ReticleMoveAnimTime = 0.1f;
        private const float ReticleFlashAnimTime = 0.2f;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _attackColor;

        public Color CurrentColor => _currentReticleColor;

        private TofuAnimation _reticleMoveAnimation;
        private TofuAnimation _reticleFlashAnimation;
        private Color _currentReticleColor;

        private Vector3 _previousInteractOffset;

        // --------------------------------------------------------------------------------------------
        private void Start()
        {
            _currentReticleColor = _defaultColor;
        }

        // --------------------------------------------------------------------------------------------
        private void AnimateInteractReticleMove(Vector3 interactOffset)
        {
            if(_previousInteractOffset.IsApproximately(interactOffset))
            {
                return;
            }

            _previousInteractOffset = interactOffset;

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

            _reticleMoveAnimation?.Stop();
            _reticleMoveAnimation = new TofuAnimation()
                .Value01(ReticleMoveAnimTime, EEaseType.Linear, (float newValue) =>
                {
                    float angle = Mathf.LerpUnclamped(fromAngle, toAngle, newValue);
                    _spriteRenderer.transform.localPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                })
                .Play();
        }

        private void AbnimateReticleColor(Color currentColor)
        {

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
            _currentReticleColor = new Color(flashColor.r, flashColor.g, flashColor.b, _defaultColor.a);

            if(_reticleFlashAnimation != null)
            {
                StopCoroutine(_reticleFlashAnimation);
            }
            _reticleFlashAnimation = StartCoroutine(AnimateReticleColorCoroutine(_currentReticleColor, flashColor));
        }

        // --------------------------------------------------------------------------------------------
        private IEnumerator AnimateReticleColorCoroutine(Color from, Color to)
        {
            float timer = 0f;
            while (timer < ReticleFlashAnimTime)
            {
                timer = Mathf.Clamp(timer + Time.deltaTime, 0f, ReticleFlashAnimTime);
                _spriteRenderer.color = Color.LerpUnclamped(from, to, TofuAnimation.Evaluate01(EEaseType.EaseOutExpo, timer / ReticleFlashAnimTime));

                yield return null;
            }

            _reticleFlashAnimation = null;
        }
    }
}
