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
            triggerFall,
            death,
            lose;

        private GameObject _listener;
        private float _musicVolume;
        private float _sfxVolume;

        private void Start()
        {
            if (Camera.main != null) _listener = Camera.main.gameObject;
            
            _musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1);
            _sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1);
            _listener.GetComponent<AudioSource>().volume = _musicVolume;
        }

        public void Shot(WeaponType weaponType, string weaponName)
        {
            switch (weaponType)
            {
                case WeaponType.Gun:
                    AudioSource.PlayClipAtPoint(gunFire, _listener.transform.position, _sfxVolume);
                    break;

                case WeaponType.Shotgun:
                    AudioSource.PlayClipAtPoint(shotgunFire, _listener.transform.position, _sfxVolume);
                    break;

                case WeaponType.Smg:
                    AudioSource.PlayClipAtPoint(smgFire, _listener.transform.position, _sfxVolume);
                    break;

                case WeaponType.Explosive:
                    AudioSource.PlayClipAtPoint(weaponName == "Zoooka" ? explosiveFire : minePlanted,
                        _listener.transform.position, _sfxVolume);

                    break;
            }
        }

        public void TriggerFall()
        {
            AudioSource.PlayClipAtPoint(triggerFall, _listener.transform.position, _sfxVolume);
        }

        public void Death()
        {
            AudioSource.PlayClipAtPoint(death, _listener.transform.position, _sfxVolume);
        }

        public void Lose()
        {
            AudioSource.PlayClipAtPoint(lose, _listener.transform.position, _sfxVolume);
        }

        public void Reload(WeaponType weaponType, string weaponName)
        {
            switch (weaponType)
            {
                case WeaponType.Gun:
                    AudioSource.PlayClipAtPoint(gunReload, _listener.transform.position, _sfxVolume);
                    break;

                case WeaponType.Shotgun:
                    AudioSource.PlayClipAtPoint(shotgunReload, _listener.transform.position, _sfxVolume);
                    break;

                case WeaponType.Smg:
                    AudioSource.PlayClipAtPoint(smgReload, _listener.transform.position, _sfxVolume);
                    break;

                case WeaponType.Explosive:
                    if (weaponName == "Zoooka")
                    {
                        AudioSource.PlayClipAtPoint(explosiveReload, _listener.transform.position, _sfxVolume);
                    }

                    break;
            }
        }
        
        public void MineDetanate()
        {
            AudioSource.PlayClipAtPoint(mineDetonate, _listener.transform.position, _sfxVolume);
        }

    }
}
