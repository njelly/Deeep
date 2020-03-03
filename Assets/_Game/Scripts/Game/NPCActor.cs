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

            PlayerActor.Instance.MoveModeChanged += PlayerActor_MoveModeChanged;
        }

        // --------------------------------------------------------------------------------------------
        private void PlayerActor_MoveModeChanged(object sender, MoveModeEventArgs e)
        {
            if(e.previousMode == PlayerActor.EMoveMode.Tactical)
            {
                
            PlayerActor.Instance.TakeTacticalTurn += PlayerActor_TakeTacticalTurn;
            }
            if(e.currentMode == PlayerActor.EMoveMode.Tactical)
            {
                PlayerActor.Instance.TakeTacticalTurn += PlayerActor_TakeTacticalTurn;
            }
        }

        // --------------------------------------------------------------------------------------------
        private void PlayerActor_TakeTacticalTurn(object sender, System.EventArgs e)
        {
            TryChooseNextTargetPosition();
            TryMoveInteractOffset();
            TryInteract();
        }
    }
}