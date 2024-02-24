using UnityEngine;

namespace Loot
{
    public class ItemRotation : MonoBehaviour
    {
        [SerializeField] private float speedInDegrees = 10;
        
        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up * speedInDegrees * Time.deltaTime, Space.Self);
        }
    }
}
