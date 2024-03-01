namespace Weapons
{
    public class SubmachineGun : Weapon
    {
        protected override void Awake()
        {
            ReloadAmmo();
            base.Awake();
        }
    }
}
