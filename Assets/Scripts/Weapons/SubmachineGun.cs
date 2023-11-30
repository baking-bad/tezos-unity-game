using Managers;
using UnityEngine;

namespace Weapons
{
    public class SubmachineGun : Weapon
    {
        private void Awake()
        {
            ReloadAmmo();
        }

        // Start is called before the first frame update
        void Start()
        {
            soundManager = GameObject.FindGameObjectWithTag("GameController")
                .GetComponent<SoundManager>();
        }
    }
}
