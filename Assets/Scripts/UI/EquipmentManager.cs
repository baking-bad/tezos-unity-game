using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using Type = Nft.NftType;

namespace UI
{
    public class EquipmentManager : MonoBehaviour
    {
        [SerializeField] private GameObject effectsPanel;
        [SerializeField] private GameObject effectRowPrefab;
        [SerializeField] private NftInventoryCreator inventoryPanel;

        private List<Nft> _equipNfts;
        private Dictionary<string, GameObject> _appliedEffects;
        
        private SlotController _slotController;

        private void Awake()
        {
            _equipNfts = new List<Nft>();
            _appliedEffects = new Dictionary<string, GameObject>();
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
            if (item.nft.Type == Nft.NftType.Armor)
            {
                var slots = _slotController.GetEquippedSlots().ToArray();
                
                foreach (var s in slots)
                {
                    var nftInventoryItem = s.gameObject.GetComponentInChildren<NftInventoryItem>();
                    if (nftInventoryItem == null) continue;
            
                    nftInventoryItem.nft.GameParameters.ForEach(param =>
                    {
                        if (!_appliedEffects.TryGetValue(param.Name, out var uiRow)) return;
                        
                        Destroy(uiRow.gameObject);
                        _appliedEffects.Remove(param.Name);
                    });
                }
                _slotController.DisableSlots(inventoryPanel.transform);
            }
            
            item.nft.GameParameters.ForEach(param =>
            {
                if (!_appliedEffects.TryGetValue(param.Name, out var uiRow)) return;
                
                Destroy(uiRow.gameObject);
                _appliedEffects.Remove(param.Name);
            });
            
            Unequip(item);
        }

        private void AddNft(NftInventoryItem item)
        {
            if (item.nft.Type == Nft.NftType.Armor)
            {
                var slotValue = item.nft.GameParameters
                    .FirstOrDefault(p => p.Name == "Slots")?.Value;

                if (slotValue == null) return;
                
                _slotController.EnableSlots((int)slotValue);
            }

            item.nft.GameParameters.ForEach(param =>
            {
                if (_appliedEffects.ContainsKey(param.Name)) return;
                
                var effect = Instantiate(effectRowPrefab, effectsPanel.transform.GetChild(0));
                _appliedEffects.Add(param.Name, effect);

                var measureType = param.MeasureType == GameParameters.Type.Unit ? "unit" : "%";
                effect.GetComponentInChildren<TMP_Text>().text = param.Name + ": +" + param.Value + " " + measureType;
            });
            
            Equip(item);
        }

        private void Equip(object item)
        {
            if (item is not NftInventoryItem module) return;
            
            _equipNfts.Add(module.nft);

            UserDataManager.Instance.SetEquipment(_equipNfts);
        }

        private void Unequip(object item)
        {
            if (item is not NftInventoryItem module) return;
            
            _equipNfts.Remove(module.nft);

            UserDataManager.Instance.SetEquipment(_equipNfts);
        }

        protected void OnDisable()
        {
            if (_slotController != null)
            {
                foreach (var slot in _slotController.slots)
                {
                    slot.ApplyNft -= AddNft;
                }
            }

            inventoryPanel.nftDropped -= RemoveNft;
        }
    }
}