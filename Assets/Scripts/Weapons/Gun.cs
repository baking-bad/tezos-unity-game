using Managers;
using UnityEngine;

namespace Weapons
{
    public class Gun : Weapon
    {
        private void Awake()
        {
            ammo = int.MaxValue;
            ReloadAmmo();
        }

        // Start is called before the first frame update
        void Start()
        {
            soundManager = GameObject.FindGameObjectWithTag("GameController")
                .GetComponent<SoundManager>();
            
            if (weaponPurpose == WeaponPurpose.Player) return;
            
            timeBtwShots = reloadTime;
        }
    }
}
