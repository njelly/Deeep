///////////////////////////////////////////////////////////////////////////////////////////////
//
//  Holdable (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/28/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    [RequireComponent(typeof(Interactable))]
    public class Holdable : MonoBehaviour
    {
        public PlayerActor HeldBy { get; private set; }

        // --------------------------------------------------------------------------------------------
        public bool canHolderRotate = true;

        // --------------------------------------------------------------------------------------------
        private void OnEnable()
        {
            Interactable interactable = GetComponent<Interactable>();
            if (interactable)
            {
                interactable.AddBeginInteractListener(OnBeginInteract);
                interactable.AddEndInteractListener(OnEndInteract);
            }
        }

        // --------------------------------------------------------------------------------------------
        private void OnDisable()
        {
            Interactable interactable = GetComponent<Interactable>();
            if (interactable)
            {
                interactable.RemoveBeginInteractListener(OnBeginInteract);
                interactable.RemoveEndInteractListener(OnEndInteract);
            }
        }

        // --------------------------------------------------------------------------------------------
        private void OnBeginInteract(Interactable.InteractedEventInfo info)
        {
            if (HeldBy != null)
            {
                return;
            }

            HeldBy.Hold(this);
        }

        // --------------------------------------------------------------------------------------------
        private void OnEndInteract(Interactable.InteractedEventInfo info)
        {
            HeldBy.Hold(null);
            HeldBy = null;
        }
    }
}