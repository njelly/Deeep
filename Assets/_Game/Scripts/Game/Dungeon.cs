///////////////////////////////////////////////////////////////////////////////////////////////
//
//  Dungeon (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/27/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using Tofunaut.SharpUnity;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class Dungeon : SharpGameObject
    {

        // --------------------------------------------------------------------------------------------
        private Dungeon(int level) : base($"Dungeon_{level}")
        {

        }

        // --------------------------------------------------------------------------------------------
        protected override void Build()
        {
            throw new System.NotImplementedException();
        }
    }
}