using System;
using Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NftType = Managers.UserDataManager.NftType;

namespace UI
{
    public class NftInventoryItem : MonoBehaviour, IPointerDownHandler
    {
        public string title;
        public string description;
        public float value;
        private string _spriteUri;
        public NftType type;
        
        [SerializeField] private Image img;

        public Action<(string, string, float, NftType, Sprite)> itemSelected;
        
        void DrawSprite(Sprite sprite)
        {
            img.sprite = sprite;
        }

        public void InitNft(
            (string nftTitle,
            string nftDescription,
            float nftValue,
            string nftSpriteUri,
            NftType nftType) nft)
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
            
            itemSelected?.Invoke((title, description, value, type, img.sprite));
            eventData.clickCount = 0;
        }
    }
}
