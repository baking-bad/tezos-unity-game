using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SlotController : MonoBehaviour
    {
        public InventorySlot[] slots;

        public void AvailableSlotsForItem(NftInventoryItem item, bool display)
        {
            foreach (var slot in slots)
            {
                if (slot.type == item.nft.Type && !slot.gameObject.GetComponentInChildren<DraggableItem>() && slot.enabled ||
                    slot.type == item.nft.Type && !display && slot.enabled)
                    slot.gameObject.GetComponent<Image>().color = display 
                        ? Color.cyan
                        : Color.white;
            }
        }

        public void EnableSlots(int count)
        {
            var moduleSlots = slots
                .Where(s => s.type == Nft.NftType.Module)
                .ToList();

            if (count > moduleSlots.Count)
            {
                count = moduleSlots.Count;
            }

            for (var i = 0; i < count; i++)
            {
                moduleSlots[i].enabled = true;
                moduleSlots[i].gameObject.GetComponent<Image>().color = Color.white;
            }
        }
        
        public void DisableSlots(Transform inventoryPanel)
        {
            var moduleSlots = slots
                .Where(s => s.type == Nft.NftType.Module)
                .ToList();
            
            foreach (var slot in moduleSlots)
            {
                slot.enabled = false;
                slot.gameObject.GetComponent<Image>().color = Color.gray;
                
                var nftItem = slot.gameObject.GetComponentInChildren<DraggableItem>();
                if (nftItem == null) continue;
                
                nftItem.parentAfterDrag = inventoryPanel;
                nftItem.transform.SetParent(inventoryPanel);
            }
        }

        public IEnumerable<InventorySlot> GetEquippedSlots()
        {
            return slots
                .Where(s => s.type == Nft.NftType.Module && 
                            s.gameObject.GetComponentInChildren<DraggableItem>());
        }
    }
}
