using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class NftInventoryCreator : MonoBehaviour, IDropHandler
    {
        [SerializeField] private GameObject nftInventoryPrefab;
        [SerializeField] private SelectedItemPanel selectedItemPanel;

        public Action<NftInventoryItem> nftDropped;

        private void Start()
        {
            UserDataManager.Instance.TokensReceived += DrawInventory;
        }

        private void DrawInventory(List<Nft> userNfts)
        {
            if (userNfts == null) return;
            
            foreach (var t in userNfts)
            {
                var item = Instantiate(nftInventoryPrefab, transform);
                item.name = nftInventoryPrefab.name;
                var script = item.GetComponentInChildren<NftInventoryItem>();
                script.InitNft(t);
                script.ItemSelected += selectedItemPanel.ShowSelectedItem;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            var dropped = eventData.pointerDrag;
            var draggableItem = dropped.GetComponent<DraggableItem>();
            
            if (draggableItem == null) return;
            
            draggableItem.parentAfterDrag = transform;
            
            nftDropped?.Invoke(dropped.GetComponent<NftInventoryItem>());
        }
        
        private void OnDisable()
        {
            UserDataManager.Instance.TokensReceived -= DrawInventory;
        }
    }
}
