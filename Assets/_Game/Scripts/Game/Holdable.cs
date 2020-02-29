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

        private Transform _previousParent;

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
            _previousParent = transform.parent;
            transform.SetParent(HeldBy.PlayerReticle.transform);
        }

        // --------------------------------------------------------------------------------------------
        public override void EndInteract(Actor instigator) 
        {
            if(_previousParent)
            {
                transform.SetParent(_previousParent);
            }
            else
            {
                transform.SetParent(null);
            }

            transform.position = HeldBy.TargetPosition + HeldBy.InteractOffset;
            HeldBy = null;
        }
    }
}