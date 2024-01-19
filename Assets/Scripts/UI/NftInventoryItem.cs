using System;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Type = Nft.NftType;

namespace UI
{
    public class NftInventoryItem : MonoBehaviour, IPointerDownHandler
    {
        public Nft nft;
        
        [SerializeField] private Image img;

        public Action<Nft, Sprite> ItemSelected;

        private ThumbnailResolver _thumbnailResolver;
        
        void DrawSprite(Sprite sprite)
        {
            img.sprite = sprite;
        }

        public void InitNft(Nft nftObj)
        {
            nft = nftObj;
            transform.parent.TryGetComponent<InventorySlot>(out var component);
            if (component != null)
            {
                component.type = nft.Type;
            }
            
            _thumbnailResolver = new ThumbnailResolver();
            _thumbnailResolver.ImageLoaded += DrawSprite;
            _thumbnailResolver.LoadThumbnail(nft.ThumbnailUri);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.clickCount != 2) return;
            
            ItemSelected?.Invoke(nft, img.sprite);
            eventData.clickCount = 0;
        }

        protected void OnDisable()
        {
            _thumbnailResolver.ImageLoaded -= DrawSprite;
        }
    }
}
