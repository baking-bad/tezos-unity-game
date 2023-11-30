using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class EquipmentManager : MonoBehaviour
    {
        [SerializeField] private GameObject effectsPanel;
        [SerializeField] private GameObject effectRowPrefab;
        [SerializeField] private NftInventoryCreator inventoryPanel;

        private Dictionary<NftInventoryItem, GameObject> _appliedNfts;
        
        private SlotController _slotController;

        private void Awake()
        {
            _appliedNfts = new Dictionary<NftInventoryItem, GameObject>();
        }

        private void Start()
        {
            TryGetComponent<SlotController>(out _slotController);
            
            if (_slotController != null)
            {
                foreach (var slot in _slotController.slots)
                {
                    slot.ApplyNft += AddNft;
                }
            }

            inventoryPanel.nftDropped += RemoveNft;
            effectsPanel.SetActive(false);
        }

        private void RemoveNft(NftInventoryItem item)
        {
            if (!_appliedNfts.TryGetValue(item, out var nftRow)) return;
            
            if (item.type == Nft.NftType.Armor)
            {
                var slots = _slotController.GetEquippedSlots().ToArray();
                
                foreach (var s in slots)
                {
                    var nft = s.gameObject.GetComponentInChildren<NftInventoryItem>();
                    if (nft == null) continue;

                    if (_appliedNfts.TryGetValue(nft, out var uiRow))
                    {
                        Destroy(uiRow.gameObject);
                    }

                    UserDataManager.Instance.Unequip(nft);
                    _appliedNfts.Remove(nft);
                }
                
                _slotController.DisableSlots(inventoryPanel.transform);
            }
            
            
            UserDataManager.Instance.Unequip(item);
            _appliedNfts.Remove(item);
            Destroy(nftRow.gameObject);
        }

        private void AddNft(NftInventoryItem item)
        {
            if (_appliedNfts.ContainsKey(item)) return;
            
            if (item.type == Nft.NftType.Armor)
            {
                _slotController.EnableSlots((int)item.value);
            }

            var effect = Instantiate(effectRowPrefab, effectsPanel.transform.GetChild(0));
            _appliedNfts.Add(item, effect);
            effect.GetComponentInChildren<TMP_Text>().text =
                item.type is Nft.NftType.Module or Nft.NftType.Ability
                    ? item.title + ": +" + item.value + "%"
                    : item.type + ": " + item.title;
            
            UserDataManager.Instance.Equip(item);
        }
    }
}