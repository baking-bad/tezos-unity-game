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
        [SerializeField] private Transform parentTransform;
        [SerializeField] private SelectedItemPanel selectedItemPanel;

        public Action<NftInventoryItem> NftDropped;

        protected virtual void Start()
        {
            UserDataManager.Instance.TokensReceived += DrawInventory;
        }

        protected void DrawInventory(List<Nft> userNfts)
        {
            if (userNfts == null) return;
            
            foreach(Transform child in parentTransform)
            {
                Destroy(child.gameObject);
            }
            
            foreach (var nft in userNfts)
            {
                var nftGameObject = Instantiate(nftInventoryPrefab, parentTransform);
                nftGameObject.name = nftInventoryPrefab.name;
                var nftScript = nftGameObject.GetComponentInChildren<NftInventoryItem>();
                nftScript.InitNft(nft);
                if (selectedItemPanel != null)
                    nftScript.ItemSelected += selectedItemPanel.ShowSelectedNft;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            var dropped = eventData.pointerDrag;
            var draggableItem = dropped.GetComponent<DraggableItem>();
            
            if (draggableItem == null) return;
            
            draggableItem.parentAfterDrag = parentTransform;
            
            NftDropped?.Invoke(dropped.GetComponent<NftInventoryItem>());
        }
        
        private void OnDisable()
        {
            UserDataManager.Instance.TokensReceived -= DrawInventory;
        }
    }
}
