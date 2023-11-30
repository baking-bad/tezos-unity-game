using System;
using Type = Nft.NftType;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class InventorySlot : MonoBehaviour, IDropHandler
    {
        public Type type;

        public Action<NftInventoryItem> ApplyNft;

        public void OnDrop(PointerEventData eventData)
        {
            var dropped = eventData.pointerDrag;
            var nftItem = eventData.pointerDrag.GetComponent<NftInventoryItem>();
            
            if (type != nftItem.type || gameObject.GetComponentInChildren<DraggableItem>()) return;

            var draggableItem = dropped.GetComponent<DraggableItem>();
            draggableItem.parentAfterDrag = transform;

            ApplyNft?.Invoke(nftItem);
        }
    }
}
