using System;
using Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Type = Nft.NftType;

namespace UI
{
    public class NftInventoryItem : MonoBehaviour, IPointerDownHandler
    {
        public string title;
        public string description;
        public float value;
        private string _spriteUri;
        public Type type;
        
        [SerializeField] private Image img;

        public Action<(string, string, Sprite)> itemSelected;
        
        void DrawSprite(Sprite sprite)
        {
            img.sprite = sprite;
        }

        public void InitNft(
            (string nftTitle,
            string nftDescription,
            float nftValue,
            string nftSpriteUri,
            Type nftType) nft)
        {
            (title, description, value, _spriteUri, type) = 
                (nft.nftTitle, nft.nftDescription, nft.nftValue, nft.nftSpriteUri, nft.nftType);

            transform.parent.TryGetComponent<InventorySlot>(out var component);
            if (component != null)
            {
                component.type = type;
            }

            var cacheHelper = new CacheHelper();
            cacheHelper.ImageLoaded += DrawSprite;

            if (cacheHelper.HasCacheImage(_spriteUri))
            {
                cacheHelper.LoadImageFromCache(_spriteUri);
            }
            else
            {
                StartCoroutine(cacheHelper.LoadImageFromUrl(_spriteUri));
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.clickCount != 2) return;
            
            itemSelected?.Invoke((title, description, img.sprite));
            eventData.clickCount = 0;
        }
    }
}
