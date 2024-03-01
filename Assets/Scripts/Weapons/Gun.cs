namespace Weapons
{
    public class Gun : Weapon
    {
        protected override void Awake()
        {
            ammo = int.MaxValue;
            ReloadAmmo();
            base.Awake();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (weaponPurpose == WeaponPurpose.Player) return;
            
            timeBtwShots = reloadTime;
        }
    }
}