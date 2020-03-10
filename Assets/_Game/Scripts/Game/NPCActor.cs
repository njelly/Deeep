///////////////////////////////////////////////////////////////////////////////////////////////
//
//  NPCActor (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/28/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class NPCActor : Actor
    {
        // --------------------------------------------------------------------------------------------
        protected override void Start()
        {
            base.Start();

            if (PlayerActor.Instance)
            {
                PlayerActor.Instance.AddMoveModeChangedListener(OnMoveModeChanged);
            }
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void OnDestroy()
        {
            if(PlayerActor.Instance)
            {
                PlayerActor.Instance.RemoveMoveModeChangedListener(OnMoveModeChanged);
            }
        }

        // --------------------------------------------------------------------------------------------
        private void OnMoveModeChanged(PlayerActor.MoveModeChangedInfo info)
        {
            if (info.previousMode == PlayerActor.EMoveMode.Tactical)
            {
                if (PlayerActor.Instance)
                {
                    PlayerActor.Instance.RemoveTakeTacticalTurnListener(OnTakeTacticalTurn);
                }
            }
            if(info.currentMode == PlayerActor.EMoveMode.Tactical)
            {
                if (PlayerActor.Instance)
                {
                    PlayerActor.Instance.AddTakeTacticalTurnListener(OnTakeTacticalTurn);
                }
            }
        }

        // --------------------------------------------------------------------------------------------
        private void OnTakeTacticalTurn()
        {
            TryChooseNextTargetPosition();
            TryMoveInteractOffset();
            TryInteract();
        }
    }
}