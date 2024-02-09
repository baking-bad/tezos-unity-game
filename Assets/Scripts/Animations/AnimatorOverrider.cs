using UnityEngine;

namespace Animations
{
    public class AnimatorOverrider : MonoBehaviour
    {
        private Animator _animator;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetAnimations(AnimatorOverrideController overrideController)
        {
            _animator.runtimeAnimatorController = overrideController;
        }
    }
}
