using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float health;
    private Rigidbody _rb;
    private Vector3 _movement;
    private Ray _ray;
    private RaycastHit _hit;

    [SerializeField] private TMP_Text healthDisplay;

    // Start is called before the first frame update
    void Start()
    {
        // _rb = GetComponent<Rigidbody>();
        healthDisplay.text = "HP: " + health;
    }

    // Update is called once per frame
    void Update()
    {
        _movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(_ray, out _hit))
        {
            transform.LookAt(new Vector3(_hit.point.x, transform.position.y, _hit.point.z));
        }
        
        transform.Translate(_movement * moveSpeed * Time.deltaTime, Space.World);

        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void ChangeHealth(float healthValue)
    {
        health += healthValue;
        healthDisplay.text = "HP: " + health;
    }
}
