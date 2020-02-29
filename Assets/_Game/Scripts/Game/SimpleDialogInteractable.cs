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
        [TextArea]
        public string dialog;

        // --------------------------------------------------------------------------------------------
        public override void BeginInteract(Actor instigator)
        {
            Debug.Log(dialog);
        }

        // --------------------------------------------------------------------------------------------
        public override void EndInteract(Actor instigator) { }
    }
}