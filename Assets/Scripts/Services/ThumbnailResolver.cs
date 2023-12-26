using System;
using Helpers;
using UnityEngine;

namespace Services
{
    public class ThumbnailResolver
    {
        public string IpfsGatewayUri = "https://ipfs.io/ipfs";
        
        public Action<Sprite> ImageLoaded;

        private string RemovePrefix(string s, string prefix) =>
            s.StartsWith(prefix) ? s.Substring(prefix.Length) : s;

        private string RemoveIpfsPrefix(string url) =>
            RemovePrefix(url, "ipfs://");

        private bool HasIpfsPrefix(string url) =>
            url?.StartsWith("ipfs://") ?? false;

        public void LoadThumbnail(string uri)
        {
            if (HasIpfsPrefix(uri))
            {
                uri = $"{IpfsGatewayUri}/" + RemoveIpfsPrefix(uri);
            }

            CacheHelper.Instance.LoadImage(uri, sprite => ImageLoaded?.Invoke(sprite));
        }
    }
}
