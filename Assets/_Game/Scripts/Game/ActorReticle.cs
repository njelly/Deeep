///////////////////////////////////////////////////////////////////////////////////////////////
//
//  ActorReticle (c) 2020 Tofunaut
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
    public class ActorReticle : MonoBehaviour
    {

        [SerializeField] private Actor _actor;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _attackColor;
        [SerializeField] private float _moveAnimTime;
        [SerializeField] private float _colorAnimTime;
        [SerializeField] private float _alphaAnimTime;

        private TofuAnimation _reticleMoveAnimation;
        private TofuAnimation _reticleColorAnimation;
        private TofuAnimation _reticleAlphaAnimation;
        private Color _currentColor;
        private Color _targetColor;
        private float _targetAlpha;
        private Vector3 _previousInteractOffset;

        // --------------------------------------------------------------------------------------------
        private void Start()
        {
            _currentColor = _defaultColor;
            _targetColor = _defaultColor;
            _targetAlpha = _defaultColor.a;
            _previousInteractOffset = _actor.InteractOffset;
        }

        // --------------------------------------------------------------------------------------------
        private void Update()
        {
            UpdateReticleMove();

            UpdateReticleColor();
            UpdateReticleAlpha();
            _spriteRenderer.color = _currentColor;
        }

        // --------------------------------------------------------------------------------------------
        private void UpdateReticleMove()
        {
            if (_previousInteractOffset.IsApproximately(_actor.InteractOffset))
            {
                return;
            }

            _previousInteractOffset = _actor.InteractOffset;

            float fromAngle = Mathf.Atan2(_spriteRenderer.transform.localPosition.x, -_spriteRenderer.transform.localPosition.y) - Mathf.PI / 2f;
            float toAngle = Mathf.Atan2(_actor.InteractOffset.x, -_actor.InteractOffset.y) - Mathf.PI / 2f;

            if (toAngle - fromAngle > Mathf.PI)
            {
                toAngle -= Mathf.PI * 2f;
            }
            if (fromAngle - toAngle > Mathf.PI)
            {
                fromAngle -= Mathf.PI * 2f;
            }

            _reticleMoveAnimation?.Stop();

            if(_reticleMoveAnimation != null)
            {
                Debug.Log("stop!");
            }

            _reticleMoveAnimation = new TofuAnimation()
                .Value01(_moveAnimTime, EEaseType.Linear, (float newValue) =>
                {
                    float angle = Mathf.LerpUnclamped(fromAngle, toAngle, newValue);
                    _spriteRenderer.transform.localPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                })
                .Then()
                .Execute(() =>
                {
                    _reticleMoveAnimation = null;
                })
                .Play();
        }

        // --------------------------------------------------------------------------------------------
        private void UpdateReticleColor()
        {
            Color changeTo = _defaultColor;

            bool facingDestructible = false;
            Collider2D[] facingColliders = Physics2D.OverlapCircleAll(transform.position + _actor.InteractOffset, 0.4f);
            foreach(Collider2D collider in facingColliders)
            {
                facingDestructible |= collider.gameObject != _actor.gameObject && collider.gameObject.GetComponent<Destructible>();
            }

            if(facingDestructible)
            {
                changeTo = _attackColor;
            }

            if(!((Vector4)changeTo).IsApproximately(_targetColor))
            {
                Color startColor = _spriteRenderer.color;
                _targetColor = changeTo;

                _reticleColorAnimation?.Stop();
                _reticleColorAnimation = new TofuAnimation()
                .Value01(_colorAnimTime, EEaseType.EaseOutExpo, (float newValue) =>
                {
                    _currentColor = Color.LerpUnclamped(startColor, _targetColor, newValue);
                })
                .Then()
                .Execute(() =>
                {
                    _reticleColorAnimation = null;
                })
                .Play();
            }
        }

        // --------------------------------------------------------------------------------------------
        private void UpdateReticleAlpha()
        {
            float alpha = _defaultColor.a;

            if(_actor.Input.space)
            {
                alpha = 1f;
            }

            if(!alpha.IsApproximately(_targetAlpha))
            {
                _targetAlpha = alpha;
                float startAlpha = _spriteRenderer.color.a;

                _reticleAlphaAnimation?.Stop();
                _reticleAlphaAnimation = new TofuAnimation()
                    .Value01(_alphaAnimTime, EEaseType.Linear, (float newValue) =>
                    {
                        _currentColor.a = Mathf.LerpUnclamped(startAlpha, _targetAlpha, newValue);
                    })
                .Then()
                .Execute(() =>
                {
                    _reticleAlphaAnimation = null;
                })
                .Play();
            }
        }
    }
}
