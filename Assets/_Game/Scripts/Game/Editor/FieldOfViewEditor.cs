///////////////////////////////////////////////////////////////////////////////////////////////
//
//  FieldOfViewEditor (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 03/07/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Tofunaut.Deeep.Game
{
    [CustomEditor (typeof(FieldOfView))]
    public class FieldOfViewEditor : Editor
    {
        public void OnSceneGUI()
		{
            FieldOfView fov = (FieldOfView)target;
            Handles.color = Color.white;
            Handles.DrawWireArc(fov.transform.position, Vector3.forward, Vector3.up, 360, fov.viewRadius);

            Handles.color = Color.red;
            foreach(Collider2D target in fov.VisibleTargets)
            {
                Handles.DrawLine(fov.transform.position, target.transform.position);
            }
        }
    }
}