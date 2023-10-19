using UnityEngine;
using WeaponType = Gun.WeaponType;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip shot, takeDamage, death, lose, shotgunShot;
    private GameObject _listener;

    private void Start()
    {
        if (Camera.main != null) _listener = Camera.main.gameObject;
    }

    public void Shot(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Gun:
                AudioSource.PlayClipAtPoint(shot, _listener.transform.position);
                break;
            
            case WeaponType.Shotgun:
                AudioSource.PlayClipAtPoint(shotgunShot, _listener.transform.position);
                break;
            
            case WeaponType.Mortar:
                break;
            
            case WeaponType.Default:
                AudioSource.PlayClipAtPoint(shot, _listener.transform.position);
                break;
            
            default:
                break;
        }
    }
    
    public void TakeDamage()
    {
        AudioSource.PlayClipAtPoint(takeDamage, _listener.transform.position);
    }
    
    public void Death()
    {
        AudioSource.PlayClipAtPoint(death, _listener.transform.position);
    }
    
    public void Lose()
    {
        AudioSource.PlayClipAtPoint(lose, _listener.transform.position);
    }
}
