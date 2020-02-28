///////////////////////////////////////////////////////////////////////////////////////////////
//
//  Interactable (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/28/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public abstract class Interactable : MonoBehaviour
    {
        // --------------------------------------------------------------------------------------------
        public abstract void Interact(Actor instigator);
    }
}