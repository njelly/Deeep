using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    public class DoorInteractable : Interactable
    {
        [Header("Door")]
        [SerializeField] protected Animator _animator;
        [SerializeField] protected AnimatorOverrideController _overrideController;
        [SerializeField] protected bool _isOpen;

        // --------------------------------------------------------------------------------------------
        private void Awake()
        {
            if(_overrideController)
            {
                _animator.runtimeAnimatorController = _overrideController;
            }

            gameObject.layer = _isOpen ? LayerMask.NameToLayer("Floor") : LayerMask.NameToLayer("Blocking");
            _animator.SetBool("up_down", DetermineIsUpDown());
            _animator.SetBool("open", _isOpen);
        }

        // --------------------------------------------------------------------------------------------
        public override void BeginInteract(Actor instigator)
        {
            _isOpen = !_isOpen;
            gameObject.layer = _isOpen ? LayerMask.NameToLayer("Floor") : LayerMask.NameToLayer("Blocking");
            _animator.SetBool("open", _isOpen);
        }

        // --------------------------------------------------------------------------------------------
        public override void EndInteract(Actor instigator) { }

        // --------------------------------------------------------------------------------------------
        private bool DetermineIsUpDown()
        {
            // for the purposes of animating, determine which way an actor should pass through the door by checking
            // if the space above and below it is blocked.
            return Physics2D.OverlapCircleAll(transform.position + Vector3.left, 0.4f, LayerMask.GetMask("Blocking")).Length > 0
                && Physics2D.OverlapCircleAll(transform.position + Vector3.right, 0.4f, LayerMask.GetMask("Blocking")).Length > 0;
        }
    }
}