using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private InventorySlot[] slots;
        
        public void AvailableSlotsForItem(NftInventoryItem item, bool display)
        {
            foreach (var slot in slots)
            {
                if (slot.type == item.type && !slot.gameObject.GetComponentInChildren<DraggableItem>() ||
                    slot.type == item.type && !display)
                    slot.gameObject.GetComponent<Image>().color = display 
                        ? Color.cyan
                        : Color.white;
            }
        }
    }
}
