 using UnityEngine;

public class Gun : MonoBehaviour
{
    public GunType gunType;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shotPoint;
    
    [SerializeField] private float startTimeBtwShots;
    private float _timeBtwShots;
    
    public enum GunType
    {
        Default,
        Enemy
    }

    private SoundManager _soundManager;

    // Start is called before the first frame update
    void Start()
    {
        _soundManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_timeBtwShots <= 0)
        {
            if (Input.GetMouseButton(0) && gunType == GunType.Default
                || gunType == GunType.Enemy)
            {
                Shoot(); 
            }
        }
        else
        {
            _timeBtwShots -= Time.deltaTime;
        }
    }

    private void Shoot()
    {
        Instantiate(bullet, shotPoint.position, shotPoint.rotation);
        _soundManager.Shot();
        _timeBtwShots = startTimeBtwShots;
    }
}
