using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    public class DoorInteractable : Interactable
    {
        [Header("Door")]
        [SerializeField] protected Animator _animator;
        [SerializeField] protected AnimatorOverrideController _overrideController;
        [SerializeField] protected bool _isOpen;
        public bool isUpDown = true;

        private void Awake()
        {
            if(_overrideController)
            {
                _animator.runtimeAnimatorController = _overrideController;
            }

            gameObject.layer = _isOpen ? LayerMask.NameToLayer("Floor") : LayerMask.NameToLayer("Blocking");
            _animator.SetBool("up_down", isUpDown);
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
    }
}