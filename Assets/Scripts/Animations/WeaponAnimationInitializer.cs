using UnityEngine;

namespace Animations
{
    public class WeaponAnimationInitializer : MonoBehaviour
    {
        [SerializeField] private AnimatorOverrideController overrideController;
        [SerializeField] private AnimatorOverrider overrider;

        public void Set()
        {
            overrider.SetAnimations(overrideController);
        }
    }
}