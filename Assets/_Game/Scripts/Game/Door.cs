///////////////////////////////////////////////////////////////////////////////////////////////
//
//  DoorInteractable (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 03/07/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    [RequireComponent(typeof(Interactable))]
    public class Door : MonoBehaviour
    {
        [Header("Door")]
        [SerializeField] protected Animator _animator;
        [SerializeField] protected AnimatorOverrideController _overrideController;
        [SerializeField] protected bool _isOpen;

        // --------------------------------------------------------------------------------------------
        private void Awake()
        {
            if (_overrideController)
            {
                _animator.runtimeAnimatorController = _overrideController;
            }

            SetLayer();
            _animator.SetBool("up_down", DetermineIsUpDown());
            _animator.SetBool("open", _isOpen);
        }

        // --------------------------------------------------------------------------------------------
        private void Start()
        {
            Interactable interactable = GetComponent<Interactable>();
            if (interactable)
            {
                interactable.AddBeginInteractListener(OnBeginInteract);
            }
        }

        // --------------------------------------------------------------------------------------------
        private void OnDestroy()
        {
            Interactable interactable = GetComponent<Interactable>();
            if (interactable)
            {
                interactable.RemoveBeginInteractListener(OnBeginInteract);
            }
        }

        // --------------------------------------------------------------------------------------------
        protected virtual void OnBeginInteract(Interactable.InteractedEventInfo info)
        {
            _isOpen = !_isOpen;
            SetLayer();
            _animator.SetBool("open", _isOpen);
        }

        // --------------------------------------------------------------------------------------------
        private bool DetermineIsUpDown()
        {
            // for the purposes of animating, determine which way an actor should pass through the door by checking
            // if the space above and below it is blocked.
            return Physics2D.OverlapCircleAll(transform.position + Vector3.left, 0.4f, LayerMask.GetMask("Blocking")).Length > 0
                && Physics2D.OverlapCircleAll(transform.position + Vector3.right, 0.4f, LayerMask.GetMask("Blocking")).Length > 0;
        }

        // --------------------------------------------------------------------------------------------
        private void SetLayer()
        {
            gameObject.layer = _isOpen ? LayerMask.NameToLayer("PassThrough") : LayerMask.NameToLayer("Blocking");
        }
    }
}