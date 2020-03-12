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
        public static HUDDialog HUDDialog => _instance._hudDialog;
        public static HUDMoveMode HUDMoveMode => _instance._hudMoveMode;
        public static Canvas Canvas => _instance._canvas;

        [SerializeField] private HUDDialog _hudDialog;
        [SerializeField] private HUDMoveMode _hudMoveMode;
        [SerializeField] private Canvas _canvas;

        protected override bool SetDontDestroyOnLoad { get { return true; } }
    }
}