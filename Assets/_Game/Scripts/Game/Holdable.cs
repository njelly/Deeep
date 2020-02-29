///////////////////////////////////////////////////////////////////////////////////////////////
//
//  Holdable (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/28/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class Holdable : Interactable
    {
        public PlayerActor HeldBy { get; private set; }

        // --------------------------------------------------------------------------------------------
        public override void BeginInteract(Actor instigator)
        {
            if(HeldBy != null)
            {
                return;
            }

            if(!(instigator is PlayerActor))
            {
                return;
            }

            HeldBy = instigator as PlayerActor;
            HeldBy.Hold(this);
        }

        // --------------------------------------------------------------------------------------------
        public override void EndInteract(Actor instigator) 
        {
            HeldBy.Hold(null);
            HeldBy = null;
        }
    }
}