///////////////////////////////////////////////////////////////////////////////////////////////
//
//  DestructibleEditor (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 03/11/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    [CustomEditor(typeof(Destructible))]
    public class DestructibleEditor : Editor
    {
        private bool _showDebugControls;
        private EDamageType _debugDamageType;

        // --------------------------------------------------------------------------------------------
        public override void OnInspectorGUI()
        {
            if(EditorApplication.isPlaying)
            {
                string showHideButtonLabel = _showDebugControls ? "Hide Debug Tools" : "Show Debug Tools";
                if (GUILayout.Button(showHideButtonLabel))
                {
                    _showDebugControls = !_showDebugControls;
                }

                if (_showDebugControls)
                {
                    Destructible destructible = (Destructible)target;

                    _debugDamageType = (EDamageType)EditorGUILayout.EnumPopup(_debugDamageType);

                    if (GUILayout.Button("Take Damage"))
                    {
                        destructible.TakeDamage(_debugDamageType);
                    }
                }
            }

            base.OnInspectorGUI();
        }
    }
}