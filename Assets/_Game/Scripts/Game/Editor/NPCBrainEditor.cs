///////////////////////////////////////////////////////////////////////////////////////////////
//
//  NPCBrainEditor (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 03/07/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;
using Tofunaut.Core;
using Tofunaut.UnityUtils;

namespace Tofunaut.Deeep.Game
{
    [CustomEditor(typeof(NPCBrain))]
    public class NPCBrainEditor : Editor
    {
        public void OnSceneGUI()
        {
            NPCBrain brain = (NPCBrain)target;
            if (!brain)
            {
                return;
            }

            Handles.color = Color.green;

            Vector3 prevPos = brain.transform.position;
            foreach (IntVector2 coord in brain.CurrentPath)
            {
                Vector3 curPos = coord.ToUnityVector3_XY();
                Handles.DrawLine(prevPos, curPos);
                prevPos = curPos;
            }
        }
    }
}