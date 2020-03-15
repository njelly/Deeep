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
    public class LockedDoor : Door
    {
        [Header("Locked")]
        public KeyItem key;
        [TextArea] public string lockedMessage;
        [TextArea] public string needKeyToCloseMessage;

        // --------------------------------------------------------------------------------------------
        protected override void OnBeginInteract(Interactable.InteractedEventInfo info)
        {
            if (key && info.instigator.Inventory)
            {
                if (!info.instigator.Inventory.ContainsItem(key))
                {
                    HUDManager.HUDDialog.ShowDialog(_isOpen ? needKeyToCloseMessage : lockedMessage);
                }
                else
                {
                    base.OnBeginInteract(info);
                }
            }
        }
    }
}