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
    public class PlayerActorView : ActorView
    {
        private const float ReticleMoveAnimTime = 0.1f;
        private const float ReticleFlashAnimTime = 0.4f;

        [Header("Player")]
        [SerializeField] private SpriteRenderer _interactReticle;

        private TofuAnimation _reticleMoveAnimation;
        private TofuAnimation _reticleFlashAnimation;
        private Color _defaultReticleColor;
        private Color _currentReticleColor;

        private Vector3 _previousInteractOffset;

        // --------------------------------------------------------------------------------------------
        protected override void Start()
        {
            base.Start();

            _defaultReticleColor = _interactReticle.color;
            _currentReticleColor = _defaultReticleColor;
        }

        // --------------------------------------------------------------------------------------------
        private void Update()
        {
            if(_actor == null)
            {
                return;
            }

            if (_actor.Input.space.Pressed)
            {
                AnimateInteractReticleColor();
            }
            else if(_actor.Input.space.Released)
            {
                AnimateInteractReticleColor(_currentReticleColor);
            }

            if (!_previousInteractOffset.IsApproximately(_actor.InteractOffset))
            {
                AnimateInteractReticleMove(_actor.InteractOffset);
                _previousInteractOffset = _actor.InteractOffset;
            }
        }

        // --------------------------------------------------------------------------------------------
        public void AnimateInteractReticleMove(Vector3 interactOffset)
        {
            _reticleMoveAnimation?.Stop();

            float fromAngle = Mathf.Atan2(_interactReticle.transform.localPosition.x, -_interactReticle.transform.localPosition.y) - Mathf.PI / 2f;
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
                    _interactReticle.transform.localPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                })
                .Play();
        }

        // --------------------------------------------------------------------------------------------
        private void AnimateInteractReticleColor()
        {
            _interactReticle.color = _currentReticleColor;
            AnimateInteractReticleColor(1f);
        }
        private void AnimateInteractReticleColor(float alpha) => AnimateInteractReticleColor(new Color(_currentReticleColor.r, _currentReticleColor.g, _currentReticleColor.b, alpha));
        private void AnimateInteractReticleColor(Color flashColor)
        {
            _currentReticleColor = new Color(flashColor.r, flashColor.g, flashColor.b, _defaultReticleColor.a);

            _reticleFlashAnimation?.Stop();
            _reticleFlashAnimation = new TofuAnimation()
                .Value01(ReticleFlashAnimTime, EEaseType.EaseOutExpo, (float newValue) =>
                {
                    _interactReticle.color = Color.LerpUnclamped(_currentReticleColor, flashColor, newValue);
                })
                .Then()
                .Execute(() =>
                {
                    _reticleFlashAnimation = null;
                })
                .Play();
        }

        // --------------------------------------------------------------------------------------------
        private void Actor_OnInteract(object sender, EventArgs e)
        {

        }
    }
}
