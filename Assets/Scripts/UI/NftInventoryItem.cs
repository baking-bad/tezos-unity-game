using System;
using Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NftInventoryItem : MonoBehaviour
    {
        private string _title;
        private string _description;
        private string _value;
        private string _spriteUri;
        
        [SerializeField] private Image img;

        public Action<(string, string, string, Sprite)> itemSelected;
        
        void DrawSprite(Sprite sprite)
        {
            img.sprite = sprite;
        }

        public void InitNft(
            (string nftTitle,
            string nftDescription,
            string nftValue,
            string nftSpriteUri) nft)
        {
            (_title, _description, _value, _spriteUri) = (nft.nftTitle, nft.nftDescription, nft.nftValue, nft.nftSpriteUri);
            
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

        public void ShowNftInfo()
        {
            itemSelected?.Invoke((_title, _description, _value, img.sprite));
        }
    }
}
