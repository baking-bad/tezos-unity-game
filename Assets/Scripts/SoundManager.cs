using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip shot, takeDamage, death, lose;
    private GameObject _listener;
    
    private void Start()
    {
        if (Camera.main != null) _listener = Camera.main.gameObject;
    }

    public void Shot()
    {
        AudioSource.PlayClipAtPoint(shot, _listener.transform.position);
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
