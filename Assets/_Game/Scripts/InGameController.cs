///////////////////////////////////////////////////////////////////////////////////////////////
//
//  InGameController (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/27/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Tofunaut.UnityUtils;

namespace Tofunaut.Deeep
{
    // --------------------------------------------------------------------------------------------
    public class InGameController : ControllerBehaviour
    {
        private Game.Game _game;

        // --------------------------------------------------------------------------------------------
        private void OnEnable()
        {
            // TODO: load saved data from file
            _game = new Game.Game(DateTime.Now.Millisecond, 0);
            _game.Render(transform);
        }

        // --------------------------------------------------------------------------------------------
        protected override void OnDisable()
        {
            base.OnDisable();

            _game.Destroy();
        }
    }

    // --------------------------------------------------------------------------------------------
    public class InGameControllerCompletedEventArgs : ControllerCompletedEventArgs
    {
        public enum Intention
        {
            StartMenu,
            QuitApp,
        }

        public readonly Intention intention;

        // --------------------------------------------------------------------------------------------
        public InGameControllerCompletedEventArgs(Intention intention, bool successfull) : base(successfull)
        {
            this.intention = intention;
        }
    }
}