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
        void Awake()
        {
            var userDataManager = GameObject.FindGameObjectWithTag("Manager")
                .GetComponent<UserDataManager>();

            userDataManager.nftsReceived += DrawInventory;
        }

        private void DrawInventory(List<Nft> userNfts)
        {
            foreach (var t in userNfts)
            {
                var item = Instantiate(nftInventoryPrefab, transform);
                var script = item.GetComponentInChildren<NftInventoryItem>();
                script.InitNft((t.Name, t.Description, t.Value, t.ThumbnailUri, t.Type));
                script.itemSelected += selectedItemPanel.ShowSelectedItem;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            var dropped = eventData.pointerDrag;
            
            var draggableItem = dropped.GetComponent<DraggableItem>();
            draggableItem.parentAfterDrag = transform;
            
            nftDropped?.Invoke(dropped.GetComponent<NftInventoryItem>());
        }
    }
}
