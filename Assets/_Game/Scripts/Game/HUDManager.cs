///////////////////////////////////////////////////////////////////////////////////////////////
//
//  HUDManager (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/29/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class HUDManager : SingletonBehaviour<HUDManager>
    {
        [SerializeField] private HUDDialog _hudDialog;

        public static HUDDialog HUDDialog => _instance._hudDialog;

        protected override bool SetDontDestroyOnLoad { get { return true; } }
    }
}