using System;
using System.Collections;
using Api.Models;
using Dynamic.Json;
using Helpers;
using UnityEngine;

namespace Api
{
    public class GameApi
    {
        private string _apiUri;

        public GameApi(string apiUri)
        {
            _apiUri = apiUri;
        }

        public IEnumerator GetPayload(
            string publicKey,
            Action<string> callback)
        {
            var routine = HttpHelper.GetRequest<string>(
                $"{_apiUri}/payload/get/?pub_key={publicKey}");
            yield return routine;

            if (routine.Current is not DJsonObject dJsonObject)
            {
                yield break;
            }

            var result = JsonUtility.FromJson<SignPayload>(dJsonObject.ToString());
            callback?.Invoke(result.payload);
        }
        
        public IEnumerator VerifyPayload(
            string publicKey,
            string signature,
            Action<bool> callback)
        {
            var routine = HttpHelper.GetRequest<string>(
                $"{_apiUri}/payload/verify/?pub_key={publicKey}&signature={signature}");
            yield return routine;

            if (routine.Current is not DJsonObject dJsonObject)
            {
                callback?.Invoke(false);
                yield break;
            }
    
            var response = JsonUtility.FromJson<SignPayloadResult>(dJsonObject.ToString());
            callback?.Invoke(response.result.Contains("Success"));
        }
    }
}
