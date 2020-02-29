///////////////////////////////////////////////////////////////////////////////////////////////
//
//  HUDDialog (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/27/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class HUDDialog : HUDModule
    {
        [Header("Dialog")]
        public float typewriterTickTime;

        [SerializeField] private TMPro.TextMeshProUGUI _text;
        [SerializeField] private GameObject _mugshotContainer;
        [SerializeField] private Image _mugshot;
        [SerializeField] private GameObject _nextPagePrompt;

        private string _dialog;
        private int _characterIndex;
        private float _typewriterTickTimer;
        private ActorInput _input;
        private Action _completeCallback;

        // --------------------------------------------------------------------------------------------
        protected override void OnEnable()
        {
            base.OnEnable();

            _characterIndex = 0;
            _typewriterTickTimer = typewriterTickTime;
            _text.text = string.Empty;
            _text.pageToDisplay = 1;
            _input = new ActorInput();
            _nextPagePrompt.SetActive(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _completeCallback?.Invoke();
            _completeCallback = null;
        }

        // --------------------------------------------------------------------------------------------
        private void Update()
        {
            _typewriterTickTimer -= Time.deltaTime;
            _input = ActorInput.PollPlayerInput(_input);

            if( _text.pageToDisplay < _text.textInfo.pageCount && _characterIndex >= _text.textInfo.pageInfo[_text.pageToDisplay - 1].lastCharacterIndex)
            {
                // there's more pages to go and we're at the end of the page
                if(_input.space.Pressed)
                {
                    _text.pageToDisplay++;
                    _nextPagePrompt.SetActive(false);
                } 
                else if(!_nextPagePrompt.activeInHierarchy)
                {
                    _nextPagePrompt.SetActive(true);
                }
            }
            else if(_typewriterTickTimer <= 0f)
            {
                _typewriterTickTimer = typewriterTickTime;
                _characterIndex++;

                string shownText = _dialog.Substring(0, Mathf.Min(_characterIndex + 1, _dialog.Length));
                string hiddenText = _characterIndex + 1 < _dialog.Length ? _dialog.Substring(_characterIndex, _dialog.Length - _characterIndex) : string.Empty;

                _text.text = $"<color=#FFFFFFFF>{shownText}</color><color=#FFFFFF00>{hiddenText}</color>";
            }

            if(_characterIndex >= _dialog.Length - 1)
            {
                if (!_nextPagePrompt.activeInHierarchy)
                {
                    _nextPagePrompt.SetActive(true);
                }

                if (_input.space.Pressed)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        // --------------------------------------------------------------------------------------------
        public void ShowDialog(string dialog) => ShowDialog(dialog, null);
        public void ShowDialog(string dialog, Action completeCallback)
        {
            // if the complete callback was previously set, invoke it now
            _completeCallback?.Invoke();
            _completeCallback = completeCallback;

            _dialog = dialog;
            gameObject.SetActive(true);
        }

        // --------------------------------------------------------------------------------------------
        public void ClearMugshot() => SetMugshot(null);
        public void SetMugshot(Sprite sprite) => SetMugshot(sprite, Color.white);
        public void SetMugshot(Sprite sprite, Color color)
        {
            _mugshot.sprite = sprite;
            _mugshot.color = color;
            _mugshotContainer.SetActive(sprite != null);
        }
    }
}