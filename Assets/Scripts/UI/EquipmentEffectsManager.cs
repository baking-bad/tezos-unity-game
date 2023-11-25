using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Type = Managers.UserDataManager.NftType;

namespace UI
{
    public class EquipmentEffectsManager : MonoBehaviour
    {
        [SerializeField] private InventorySlot[] equipmentSlots;
        [SerializeField] private GameObject effectRowPrefab;
        [SerializeField] private NftInventoryCreator inventoryPanel;

        private Dictionary<NftInventoryItem, GameObject> _appliedNfts;

        private void Awake()
        {
            _appliedNfts = new Dictionary<NftInventoryItem, GameObject>();
        }

        private void Start()
        {
            foreach (var slot in equipmentSlots)
            {
                slot.ApplyNft += AddNftEffect;
            }

            inventoryPanel.nftDropped += RemoveNftEffect;
            transform.parent.gameObject.SetActive(false);
        }

        private void RemoveNftEffect(NftInventoryItem item)
        {
            if (!_appliedNfts.TryGetValue(item, out var nftRow)) return;
            
            _appliedNfts.Remove(item);
            Destroy(nftRow.gameObject);
        }

        private void AddNftEffect(NftInventoryItem item)
        {
            if (_appliedNfts.ContainsKey(item)) return;
            
            var effect = Instantiate(effectRowPrefab, gameObject.transform);
            _appliedNfts.Add(item, effect);
            effect.GetComponentInChildren<TMP_Text>().text = item.title + ": " + item.value + "%";
        }
    }
}
