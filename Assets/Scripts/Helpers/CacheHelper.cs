using System;
using System.Collections;
using System.IO;
using Common;
using TezosSDK.Helpers;
using UnityEngine;
using UnityEngine.Networking;

namespace Helpers
{
    public class CacheHelper
    {
        private static CacheHelper _instance;
        public static CacheHelper Instance = _instance ?? new CacheHelper();

        public void LoadImage(string url, Action<Sprite> imageLoaded)
        {
            if (HasCacheImage(url))
            {
                LoadImageFromCache(url, imageLoaded);
            }
            else
            {
                CoroutineRunner.Instance.StartCoroutine(LoadImageFromUrl(url, imageLoaded));
            }
        }

        private bool HasCacheImage(string url)
        {
            var hash = Sha256.ComputeHash(url);
            return File.Exists(Application.persistentDataPath + hash);
        }

        private IEnumerator LoadImageFromUrl(string url, Action<Sprite> imageLoaded)
        {
            if (string.IsNullOrEmpty(url)) yield break;
            
            var www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            try
            {
                if (www.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Network error: " + www.error);
                }
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(www);
                    var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    imageLoaded?.Invoke(sprite);
                    SaveImageToCache(sprite, url);
                }
                www.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogError("Load image from url " + url + " error: " + e);
            }
        }

        private void LoadImageFromCache(string url, Action<Sprite> imageLoaded)
        {
            var hash = Sha256.ComputeHash(url);
            var textureBytes = File.ReadAllBytes(Application.persistentDataPath + hash);
            var loadedTexture = new Texture2D(0, 0);
            loadedTexture.LoadImage(textureBytes);
            var sprite = Sprite.Create(loadedTexture, new Rect(0, 0, loadedTexture.width, loadedTexture.height), Vector2.zero);
            imageLoaded?.Invoke(sprite);
        }

        private void SaveImageToCache(Sprite sprite, string url)
        {
            var hash = Sha256.ComputeHash(url);
            var textureBytes = sprite.texture.EncodeToPNG();
            File.WriteAllBytes(Application.persistentDataPath + hash, textureBytes);
        }
    }
}
