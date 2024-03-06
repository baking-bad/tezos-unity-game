using UnityEngine;

namespace Loot
{
    public class NftLootBox : MonoBehaviour
    {
        [SerializeField] private GameObject lootBox;
        [SerializeField] private float moveSpeed = 1;
        [SerializeField] private float destructionTime = 20;


        private void Start()
        {
            Invoke(nameof(DestroyNftBox), destructionTime);
        }

        private void Update()
        {
            lootBox.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.World);
        }

        private void DestroyNftBox()
        {
            Destroy(gameObject);
        }
    }
}
