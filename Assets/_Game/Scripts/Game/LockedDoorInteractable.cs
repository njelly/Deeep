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
    public class LockedDoorInteractable : DoorInteractable
    {
        [Header("Locked")]
        public KeyItem key;
        [TextArea] public string lockedMessage;
        [TextArea] public string needKeyToCloseMessage;

        // --------------------------------------------------------------------------------------------
        public override void BeginInteract(Actor instigator)
        {
            if(key && instigator.Inventory)
            {
                if (!instigator.Inventory.ContainsItem(key))
                {
                    HUDManager.HUDDialog.ShowDialog(_isOpen ? needKeyToCloseMessage : lockedMessage);
                }
                else
                {
                    base.BeginInteract(instigator);
                }
            }
        }

        // --------------------------------------------------------------------------------------------
        public override void EndInteract(Actor instigator) { }
    }
}