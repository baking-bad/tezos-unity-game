using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Transform _rootPanel;
        private SlotController _slotController;
        [SerializeField] private Image image;
        [HideInInspector] public Transform parentAfterDrag;

        private Vector3 _offset;
        private CanvasGroup _cg;

        private void Awake()
        {
            _cg = GetComponent<CanvasGroup>();

            if (_cg == null)
            {
                _cg = gameObject.AddComponent<CanvasGroup>();
            }
            
            _rootPanel = GameObject.FindGameObjectWithTag("Inventory-panel").transform;
            _slotController = _rootPanel.GetComponent<SlotController>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _offset = transform.position - GetMousePosition();
            
            gameObject.transform.parent.TryGetComponent<InventorySlot>(out var slot);
            if (slot != null)
            {
                if (slot.transform.childCount > 1)
                {
                    slot.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                }
            }

            parentAfterDrag = transform.parent;
            transform.SetParent(_rootPanel);
            // transform.SetAsLastSibling();
            _cg.alpha = 0.7f;
            image.raycastTarget = false;
            
            _slotController.AvailableSlotsForItem(
                item: gameObject.GetComponent<NftInventoryItem>(),
                display: true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = GetMousePosition() + _offset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.SetParent(parentAfterDrag);
            
            gameObject.transform.parent.TryGetComponent<InventorySlot>(out var slot);
            if (slot != null)
            {
                if (slot.transform.childCount > 1)
                {
                    slot.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            
            _cg.alpha = 1f;
            image.raycastTarget = true;
            
            _slotController.AvailableSlotsForItem(
                item: gameObject.GetComponent<NftInventoryItem>(), 
                display: false);
        }

        private Vector3 GetMousePosition()
        {
            var mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
            return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        }
    }
}
