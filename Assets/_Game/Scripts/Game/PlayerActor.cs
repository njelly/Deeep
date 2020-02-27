///////////////////////////////////////////////////////////////////////////////////////////////
//
//  PlayerActor (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/27/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class PlayerActor : Actor
    {

        // --------------------------------------------------------------------------------------------
        public PlayerActor() : base ("PlayerActor", AssetPaths.Prefabs.PlayerActorView) { }

        // --------------------------------------------------------------------------------------------
        protected override void UpdateInput()
        {
            _input.up.wasDown = _input.up.isDown;
            _input.up.isDown = Input.GetKey(KeyCode.UpArrow);
            _input.down.wasDown = _input.down.isDown;
            _input.down.isDown = Input.GetKey(KeyCode.DownArrow);
            _input.left.wasDown = _input.left.isDown;
            _input.left.isDown = Input.GetKey(KeyCode.LeftArrow);
            _input.right.wasDown = _input.right.isDown;
            _input.right.isDown = Input.GetKey(KeyCode.RightArrow);
            _input.shift.wasDown = _input.shift.isDown;
            _input.shift.isDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            _input.one.wasDown = _input.one.isDown;
            _input.one.isDown = Input.GetKey(KeyCode.Alpha1);
            _input.two.wasDown = _input.two.isDown;
            _input.two.isDown = Input.GetKey(KeyCode.Alpha2);
            _input.three.wasDown = _input.three.isDown;
            _input.three.isDown = Input.GetKey(KeyCode.Alpha3);
            _input.four.wasDown = _input.four.isDown;
            _input.four.isDown = Input.GetKey(KeyCode.Alpha4);
        }
    }
}