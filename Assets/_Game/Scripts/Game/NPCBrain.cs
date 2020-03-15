///////////////////////////////////////////////////////////////////////////////////////////////
//
//  NPCBrain (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 03/15/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using Tofunaut.UnityUtils;
using System.Collections.Generic;
using UnityEngine;
using Tofunaut.Core;

namespace Tofunaut.Deeep.Game
{
    [RequireComponent(typeof(Actor))]
    [RequireComponent(typeof(FieldOfView))]
    public class NPCBrain : MonoBehaviour
    {
        public LayerMask aggroMask;
        [SerializeField] protected Actor _actor;
        [SerializeField] protected FieldOfView _fieldOfView;

        public Collider2D CurrentTarget { get; private set; }
        public IntVector2[] CurrentPath => _currentPath;

        private Vector3 _prevTargetPosition;
        private IntVector2[] _currentPath;

        // SKELETONS THROW THEIR RIBS AT YOU (When angry?) - Olga
        // So maybe there's a class of skeletons with a projectile weapon? (Minecraft skellies)
        // But when they're out of ribs they die

        protected virtual void Start()
        {
            PlayerActor.Instance.AddMoveModeChangedListener(OnMoveModeChanged);
        }

        protected virtual void OnDestroy()
        {
            PlayerActor.Instance.RemoveMoveModeChangedListener(OnMoveModeChanged);
        }

        protected virtual void Update()
        {
            // always evaluate the current target (if any)
            CheckCurrentTarget();

            if (CurrentTarget)
            {
                if (!_prevTargetPosition.IsApproximately(CurrentTarget.transform.position))
                {
                    _currentPath = PathFinder.FindPath(
                        new IntVector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)),
                        new IntVector2(Mathf.RoundToInt(CurrentTarget.transform.position.x), Mathf.RoundToInt(CurrentTarget.transform.position.y)),
                        _actor);
                }

                _prevTargetPosition = CurrentTarget.transform.position.RoundToInt();
            }
        }

        protected virtual void CheckCurrentTarget()
        {
            List<Collider2D> availableTargets = new List<Collider2D>(_fieldOfView.VisibleTargets);
            if (availableTargets.Count > 0)
            {
                // choose the best target
                availableTargets.Sort((Collider2D a, Collider2D b) =>
                {
                    return EvaluateTargets(a, b);
                });

                CurrentTarget = availableTargets[0];
            }
        }

        ///<summary>
        /// Prefered targets should return LESS (-1) values.
        ///</summary>
        protected virtual int EvaluateTargets(Collider2D targetA, Collider2D targetB)
        {
            return (targetA.transform.position - transform.position).sqrMagnitude < (targetB.transform.position - transform.position).sqrMagnitude ? -1 : 1;
        }

        protected virtual void OnMoveModeChanged(PlayerActor.MoveModeChangedInfo info)
        {
            switch (info.currentMode)
            {
                case PlayerActor.EMoveMode.FreeMove:
                    PlayerActor.Instance.RemoveTakeTacticalTurnListener(OnTakeTacticalTurn);
                    break;
                case PlayerActor.EMoveMode.Tactical:
                    PlayerActor.Instance.AddTakeTacticalTurnListener(OnTakeTacticalTurn);
                    break;
            }
        }

        protected virtual void OnTakeTacticalTurn()
        {
            Debug.Log("take tactical turn!");
        }


    }
}