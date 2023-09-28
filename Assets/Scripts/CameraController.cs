using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private Vector3 targetOffset;

    [SerializeField] private float movementSpeed;
    
    void LateUpdate()
    {
        // transform.position = Vector3.SmoothDamp(transform.position, target.position, ref movementSpeed, 0.3f);
        transform.position = Vector3.Lerp(transform.position, target.position + targetOffset,
            movementSpeed * Time.deltaTime);
    }
}
