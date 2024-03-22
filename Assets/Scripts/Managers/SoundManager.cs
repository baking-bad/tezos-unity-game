using UnityEngine;
using Weapons;
using WeaponType = Weapons.Weapon.WeaponType;

namespace Managers
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClip gunFire,
            shotgunFire,
            smgFire,
            weaponReload,
            minePlanted,
            mineDetonate,
            explosiveFire,
            triggerFall,
            lose,
            roar,
            rangeFire,
            switchWeapon,
            pickup,
            drop,
            step;

        [SerializeField] private AudioClip[] groans;
        [SerializeField] private AudioClip[] deaths;

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

        public void Shot(Weapon weapon)
        {
            switch (weapon.weaponType)
            {
                case WeaponType.Gun:
                    AudioSource.PlayClipAtPoint(
                        weapon.weaponPurpose == Weapon.WeaponPurpose.Player ? gunFire : rangeFire,
                        _listener.transform.position, _sfxVolume);
                    break;

                case WeaponType.Shotgun:
                    AudioSource.PlayClipAtPoint(shotgunFire, _listener.transform.position, _sfxVolume);
                    break;

                case WeaponType.Smg:
                    AudioSource.PlayClipAtPoint(smgFire, _listener.transform.position, _sfxVolume);
                    break;

                case WeaponType.Explosive:
                    AudioSource.PlayClipAtPoint(
                        weapon.name == "Zoooka" ? explosiveFire : minePlanted,
                        _listener.transform.position, _sfxVolume);
                    break;
            }
        }
        
        public void SwitchWeapon()
        {
            AudioSource.PlayClipAtPoint(switchWeapon, _listener.transform.position, _sfxVolume);
        }

        public void TriggerFall()
        {
            AudioSource.PlayClipAtPoint(triggerFall, _listener.transform.position, _sfxVolume);
        }

        public void Death()
        {
            var rnd = Random.Range(0, deaths.Length);
            AudioSource.PlayClipAtPoint(deaths[rnd], _listener.transform.position, _sfxVolume);
        }
        
        public void Roar()
        {
            AudioSource.PlayClipAtPoint(roar, _listener.transform.position, _sfxVolume * 5);
        }
        
        public void Groan()
        {
            var rnd = Random.Range(0, groans.Length);
            AudioSource.PlayClipAtPoint(groans[rnd], _listener.transform.position, _sfxVolume);
        }

        public void Lose()
        {
            AudioSource.PlayClipAtPoint(lose, _listener.transform.position, _sfxVolume);
        }

        public void Reload()
        {
            AudioSource.PlayClipAtPoint(weaponReload, _listener.transform.position, _sfxVolume);
        }
        
        public void MineDetanate()
        {
            AudioSource.PlayClipAtPoint(mineDetonate, _listener.transform.position, _sfxVolume);
        }

        public void Drop()
        {
            AudioSource.PlayClipAtPoint(drop, _listener.transform.position, _sfxVolume * 5);
        }

        public void LootPickup()
        {
            AudioSource.PlayClipAtPoint(pickup, _listener.transform.position, _sfxVolume);
        }
    }
}
