///////////////////////////////////////////////////////////////////////////////////////////////
//
//  FieldOfView (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 03/07/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    public class FieldOfView : MonoBehaviour
    {
        public HashSet<Collider2D> VisibleTargets => new HashSet<Collider2D>(_visibleTargets) ?? new HashSet<Collider2D>();

        public LayerMask targetMask;
        public float viewRadius;

        private List<Collider2D> _visibleTargets = new List<Collider2D>();

        private void Update()
        {
            FindVisibleTargets();
        }

        private void FindVisibleTargets()
        {
            _visibleTargets.Clear();
            Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);
            foreach (Collider2D target in potentialTargets)
            {
                Vector2 toTarget = target.transform.position - transform.position;
                if (!Physics2D.Raycast(transform.position, toTarget, toTarget.magnitude, LayerMask.GetMask("Blocking")))
                {
                    _visibleTargets.Add(target);
                }
            }
        }

        public Vector2 DirFromAngle(float angleInDegrees, bool isGlobal)
        {
            if (!isGlobal)
            {
                angleInDegrees += transform.eulerAngles.z;
            }
            return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}