using Managers;
using UnityEngine;

namespace Weapons
{
    public class SubmachineGun : Weapon
    {
        private void Awake()
        {
            /*
            * Test case
            */
            ammo = 999;
            /*
            * Test case
            */
            
            ReloadAmmo();
        }

        // Start is called before the first frame update
        void Start()
        {
            soundManager = GameObject.FindGameObjectWithTag("Manager")
                .GetComponent<SoundManager>();
        }
    }
}
