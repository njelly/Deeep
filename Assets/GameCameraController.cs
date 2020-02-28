///////////////////////////////////////////////////////////////////////////////////////////////
//
//  GameCameraController (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/28/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    [RequireComponent(typeof(Camera))]
    public class GameCameraController : SingletonBehaviour<GameCameraController>
    {
        public Vector3 Offset { get; set; }

        public GameObject target;

        // --------------------------------------------------------------------------------------------
        private void Start()
        {
            Offset = transform.position - target.transform.position;
        }

        // --------------------------------------------------------------------------------------------
        private void LateUpdate()
        {
            if(!target)
            {
                return;
            }

            transform.position = target.transform.position + Offset;
        }
    }
}
