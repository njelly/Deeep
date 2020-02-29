///////////////////////////////////////////////////////////////////////////////////////////////
//
//  HUDDialog (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/27/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

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

        private string _dialog;
        private int _characterIndex;
        private float _typewriterTickTimer;
        private ActorInput _input;

        // --------------------------------------------------------------------------------------------
        protected override void OnEnable()
        {
            base.OnEnable();

            _characterIndex = 0;
            _typewriterTickTimer = typewriterTickTime;
            _text.text = string.Empty;
            _input = new ActorInput();
        }

        // --------------------------------------------------------------------------------------------
        private void Update()
        {
            _typewriterTickTimer -= Time.deltaTime;
            _input = ActorInput.PollPlayerInput(_input);

            if (_typewriterTickTimer <= 0f)
            {
                bool keepGoing = _characterIndex < _text.textInfo.pageInfo[_text.pageToDisplay].lastCharacterIndex;
                if(!keepGoing)
                {
                    keepGoing = _input.space;
                    if(keepGoing)
                    {
                        _text.pageToDisplay++;
                    }
                }

                if(keepGoing)
                {
                    _typewriterTickTimer = typewriterTickTime;
                    _characterIndex++;

                    _text.text = $"<color=#FFFFFFFF>{_dialog.Substring(0, _characterIndex)}</color><color=#FFFFFF00>{_dialog.Substring(_characterIndex, _dialog.Length - _characterIndex)}</color>";
                }
            }
        }

        // --------------------------------------------------------------------------------------------
        public void ShowDialog(string dialog)
        {
            _dialog = dialog;

            gameObject.SetActive(true);
        }

        // --------------------------------------------------------------------------------------------
        public void SetMugshot(Sprite sprite)
        {
            _mugshot.sprite = sprite;
            _mugshotContainer.SetActive(sprite != null);
        }
    }
}