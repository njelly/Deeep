///////////////////////////////////////////////////////////////////////////////////////////////
//
//  SimpleDialogInteractable (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/28/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    [RequireComponent(typeof(Interactable))]
    public class SimpleDialog : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRender;
        [TextArea] public string dialog;

        // --------------------------------------------------------------------------------------------
        private void Start()
        {
            Interactable interactable = GetComponent<Interactable>();
            if (interactable)
            {
                interactable.AddBeginInteractListener(OnBeginInteract);
            }
        }

        // --------------------------------------------------------------------------------------------
        private void OnDestroy()
        {
            Interactable interactable = GetComponent<Interactable>();
            if (interactable)
            {
                interactable.RemoveBeginInteractListener(OnBeginInteract);
            }
        }

        // --------------------------------------------------------------------------------------------
        protected void OnBeginInteract(Interactable.InteractedEventInfo info)
        {
            HUDManager.HUDDialog.SetMugshot(_spriteRender ? _spriteRender.sprite : null);
            HUDManager.HUDDialog.ShowDialog(dialog);
        }
    }
}