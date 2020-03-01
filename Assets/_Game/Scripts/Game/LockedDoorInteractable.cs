///////////////////////////////////////////////////////////////////////////////////////////////
//
//  LockedDoorInteractable (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/29/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    public class LockedDoorInteractable : Interactable
    {
        public KeyItem key;
        [TextArea] public string lockedMessage;

        // --------------------------------------------------------------------------------------------
        public override void BeginInteract(Actor instigator)
        {
            if(key && instigator.Inventory)
            {
                if (instigator.Inventory.ContainsItem(key))
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    HUDManager.HUDDialog.ShowDialog(lockedMessage);
                }
            }
        }

        // --------------------------------------------------------------------------------------------
        public override void EndInteract(Actor instigator) { }
    }
}