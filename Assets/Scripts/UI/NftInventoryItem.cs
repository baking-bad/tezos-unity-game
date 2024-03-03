using System;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Type = Nft.NftType;

namespace UI
{
    public class NftInventoryItem : MonoBehaviour, IPointerClickHandler
    {
        public Nft Nft;
        
        [SerializeField] private Image img;

        public Action<Nft, Sprite> ItemSelected;

        private ThumbnailResolver _thumbnailResolver;
        
        void DrawSprite(Sprite sprite)
        {
            img.sprite = sprite;
        }

        public void InitNft(Nft nftObj)
        {
            Nft = nftObj;
            transform.parent.TryGetComponent<InventorySlot>(out var component);
            if (component != null)
            {
                component.type = Nft.Type;
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
            
            ItemSelected?.Invoke(Nft, img.sprite);
            eventData.clickCount = 0;
        }
    }
}
