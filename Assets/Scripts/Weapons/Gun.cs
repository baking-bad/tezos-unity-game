using Managers;
using UnityEngine;

namespace Weapons
{
    public class Gun : Weapon
    {
        // Start is called before the first frame update
        void Start()
        {
            soundManager = GameObject.FindGameObjectWithTag("Manager")
                .GetComponent<SoundManager>();
            
            ReloadAmmo();
        }
    }
}
