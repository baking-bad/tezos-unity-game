using Managers;
using UnityEngine;

namespace Weapons
{
    public class SubmachineGun : Weapon
    {
        protected override void Awake()
        {
            ReloadAmmo();
            base.Awake();
        }

        // Start is called before the first frame update
        void Start()
        {
            soundManager = GameObject.FindGameObjectWithTag("GameController")
                .GetComponent<SoundManager>();
        }
    }
}
