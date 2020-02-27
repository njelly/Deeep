///////////////////////////////////////////////////////////////////////////////////////////////
//
//  Game (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/27/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using Tofunaut.SharpUnity;
using Tofunaut.UnityUtils;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class Game : SharpGameObject
    {
        public enum PlayMode
        {
            /// <summary>
            /// All actors can move freely around the world.
            /// </summary>
            FreeMove = 0,

            /// <summary>
            /// Movement is turn based, all actors wait for the player to take an action before taking their action.
            /// </summary>
            Fight = 1,

            /// <summary>
            /// No Actors can move. Used during menus.
            /// </summary>
            Paused = 2,
        }

        public PlayMode CurrentMode { get; private set; }
        public int CurrentLevel { get; private set; }

        private readonly int _seed;
        private List<Actor> _actors;
        private PlayMode _previousMode;
        private PlayerActor _playerActor;

        // --------------------------------------------------------------------------------------------
        public Game(int seed, int initalLevel) : base("Game")
        {
            _seed = seed;
            CurrentMode = PlayMode.FreeMove;
            CurrentLevel = initalLevel;

            _actors = new List<Actor>();
        }

        // --------------------------------------------------------------------------------------------
        protected override void Build()
        {
            _playerActor = new PlayerActor();
            _playerActor.Render(Transform);

            _actors.Add(_playerActor);
        }

        // --------------------------------------------------------------------------------------------
        public void SetPaused(bool paused)
        {
            if(paused)
            {
                _previousMode = CurrentMode;
                CurrentMode = PlayMode.Paused;
            }
            else
            {
                CurrentMode = _previousMode;
            }
        }
    }
}