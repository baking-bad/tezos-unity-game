using System;
using System.Collections;
using System.Text;
using System.Text.Json;
using Dynamic.Json;
using TezosSDK.Helpers;
using UnityEngine;
using UnityEngine.Networking;
using Result = UnityEngine.Networking.UnityWebRequest.Result;

namespace Helpers
{
    public static class HttpHelper
    {
        public static int RequestTimeout = 30;
        public static IEnumerator GetRequest<T>(string uri)
        {
            var request = GetUnityWebRequest(uri,UnityWebRequest.kHttpVerbGET);
            yield return request.SendWebRequest();

            if (request.result != Result.Success)
            {
                Debug.LogError($"GetRequest failed with error: {request.error}");
                request.Dispose();
                yield break;
            }
            
            if (string.IsNullOrWhiteSpace(request.downloadHandler.text))
            {
                request.Dispose();
                yield break;
            }

            if (typeof(T) == typeof(string))
            {
                yield return DJson.Parse(request.downloadHandler.text, JsonOptions.DefaultOptions);
            }
            else
            {
                yield return JsonSerializer.Deserialize<T>(request.downloadHandler.text, JsonOptions.DefaultOptions);
            }

            request.Dispose();
        }
        
        public static IEnumerator PostRequest<T>(string uri, object data)
        {
            var request = GetUnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);
            try
            {
                var serializedData = JsonSerializer.Serialize(data, JsonOptions.DefaultOptions);
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(serializedData));
                request.SetRequestHeader("Content-Type", "application/json");
                request.SendWebRequest();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            yield return new WaitUntil(() => request.isDone);
            yield return JsonSerializer.Deserialize<T>(request.downloadHandler.text, JsonOptions.DefaultOptions);
            request.Dispose();
        }

        private static UnityWebRequest GetUnityWebRequest(string uri, string method)
        {
            var request = new UnityWebRequest(uri, method);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Accept", "application/json");
            request.timeout = RequestTimeout;
            return request;
        }
    }
}
