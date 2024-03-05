using System;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace UI
{
    public class NftInventoryItem : MonoBehaviour, IPointerClickHandler
    {
        public Nft Nft;

        [SerializeField] private Image img;
        [SerializeField] private TMP_Text nftName;
        [SerializeField] private TMP_Text nftType;
        [SerializeField] private GameObject amountBadge;
        [SerializeField] private TMP_Text amountText;

        public Action<Nft, Sprite> ItemSelected;

        private ThumbnailResolver _thumbnailResolver;

        void DrawSprite(Sprite sprite)
        {
            img.sprite = sprite;
        }

        public void InitNft(Nft nftObj)
        {
            Nft = nftObj;
            transform.parent.TryGetComponent<InventorySlot>(out var inventorySlot);
            if (inventorySlot != null)
                inventorySlot.type = Nft.Type;

            if (nftName != null)
                nftName.text = Nft.Name;

            if (nftType != null)
                nftType.text = Nft.Type.ToString();

            if (amountBadge != null && amountText != null && nftObj.Amount > 1)
            {
                amountBadge.SetActive(true);
                amountText.text = $"x{nftObj.Amount}";
            }

            _thumbnailResolver = new ThumbnailResolver();
            _thumbnailResolver.ImageLoaded += DrawSprite;
            _thumbnailResolver.LoadThumbnail(Nft.ThumbnailUri);
        }

        protected void OnDisable()
        {
            _thumbnailResolver.ImageLoaded -= DrawSprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount < 2) return;

            ItemSelected?.Invoke((Nft)Nft, img.sprite);
            eventData.clickCount = 0;
        }
    }
}