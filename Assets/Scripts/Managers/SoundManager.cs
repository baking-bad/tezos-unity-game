using System;
using UnityEngine;
using WeaponType = Weapons.Weapon.WeaponType;

namespace Managers
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClip gunFire,
            gunReload,
            shotgunFire,
            shotgunReload,
            smgFire,
            smgReload,
            minePlanted,
            mineDetonate,
            explosiveFire,
            explosiveReload,
            death,
            lose;

        private GameObject _listener;

        private void Start()
        {
            if (Camera.main != null) _listener = Camera.main.gameObject;
        }

        public void Shot(WeaponType weaponType, string weaponName)
        {
            switch (weaponType)
            {
                case WeaponType.Gun:
                    AudioSource.PlayClipAtPoint(gunFire, _listener.transform.position);
                    break;

                case WeaponType.Shotgun:
                    AudioSource.PlayClipAtPoint(shotgunFire, _listener.transform.position);
                    break;

                case WeaponType.Smg:
                    AudioSource.PlayClipAtPoint(smgFire, _listener.transform.position);
                    break;

                case WeaponType.Explosive:
                    if (weaponName == "Zoooka")
                    {
                        AudioSource.PlayClipAtPoint(explosiveFire, _listener.transform.position);
                    }
                    else
                    {
                        AudioSource.PlayClipAtPoint(minePlanted, _listener.transform.position, 0.1f);
                    }

                    break;

                default:
                    break;
            }
        }

        public void Death()
        {
            AudioSource.PlayClipAtPoint(death, _listener.transform.position);
        }

        public void Lose()
        {
            AudioSource.PlayClipAtPoint(lose, _listener.transform.position);
        }

        public void Reload(WeaponType weaponType, string weaponName)
        {
            switch (weaponType)
            {
                case WeaponType.Gun:
                    AudioSource.PlayClipAtPoint(gunReload, _listener.transform.position);
                    break;

                case WeaponType.Shotgun:
                    AudioSource.PlayClipAtPoint(shotgunReload, _listener.transform.position);
                    break;

                case WeaponType.Smg:
                    AudioSource.PlayClipAtPoint(smgReload, _listener.transform.position);
                    break;

                case WeaponType.Explosive:
                    if (weaponName == "Zoooka")
                    {
                        AudioSource.PlayClipAtPoint(explosiveReload, _listener.transform.position);
                    }

                    break;
            }
        }
        
        public void MineDetanate()
        {
            AudioSource.PlayClipAtPoint(mineDetonate, _listener.transform.position);
        }

    }
}
