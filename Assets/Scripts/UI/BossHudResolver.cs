using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BossHudResolver : MonoBehaviour
    {
        [SerializeField] private Slider bossHealthBar;
        [SerializeField] private TMP_Text bossNameText;
        public Enemy owner;

        private void Start()
        {
            bossNameText.text = owner.name;
            owner.HealthChanged += HealthBarChanged;
        }

        private void HealthBarChanged(float value, float maxValue)
        {
            bossHealthBar.value = value / maxValue;
            
            if (value > 0) return;
            owner.HealthChanged -= HealthBarChanged;
            Destroy(gameObject);
        }
    }
}
