using System;
using Type = Managers.UserDataManager.NftType;
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
            if (type != dropped.GetComponent<NftInventoryItem>().type ||
                gameObject.GetComponentInChildren<DraggableItem>()) return;

            var draggableItem = dropped.GetComponent<DraggableItem>();
            draggableItem.parentAfterDrag = transform;
            ApplyNft?.Invoke(dropped.GetComponent<NftInventoryItem>());
        }
    }
}
