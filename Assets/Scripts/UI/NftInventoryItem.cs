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

        public Action<Nft, Sprite> itemSelected;
        
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
            
            var thumbnailResolver = new ThumbnailResolver();
            thumbnailResolver.ImageLoaded += DrawSprite;
            thumbnailResolver.LoadThumbnail(nft.ThumbnailUri);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.clickCount != 2) return;
            
            itemSelected?.Invoke(nft, img.sprite);
            eventData.clickCount = 0;
        }
    }
}
