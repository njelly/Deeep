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
    public class SimpleDialogInteractable : Interactable
    {
        [SerializeField] private SpriteRenderer _spriteRender;
        [TextArea] public string dialog;

        // --------------------------------------------------------------------------------------------
        public override void BeginInteract(Actor instigator)
        {
            HUDManager.HUDDialog.SetMugshot(_spriteRender ? _spriteRender.sprite : null);
            HUDManager.HUDDialog.ShowDialog(dialog);
        }

        // --------------------------------------------------------------------------------------------
        public override void EndInteract(Actor instigator) { }
    }
}