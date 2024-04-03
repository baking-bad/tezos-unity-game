using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
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
                $"{_apiUri}/payload/get/?public_key={publicKey}");
            yield return routine;

            if (routine.Current is not DJsonObject dJsonObject)
            {
                yield break;
            }
            
            var response = JsonSerializer.Deserialize<SignPayload>(dJsonObject.ToString());
            
            if (response == null)
            {
                yield break;
            }
            
            callback?.Invoke(response.Payload);
        }
        
        public IEnumerator VerifyPayload(
            string publicKey,
            string signature,
            Action<bool> callback)
        {
            var data = new
            {
                public_key = publicKey,
                signature
            };
            
            var routine = HttpHelper.PostRequest<object>(
                $"{_apiUri}/payload/verify/",
                data);
            yield return routine;
            
            if (routine.Current == null) yield break;

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(routine.Current.ToString());
            jsonElement.TryGetProperty("response", out var response);

            if (response.ValueKind == JsonValueKind.Null) yield break;
            
            var result = response.GetString()?.Contains("Success") ?? false;
            callback?.Invoke(result);
        }

        public IEnumerator CreateGameSession(
            string address,
            Action<GameSession> callback)
        {
            
            var data = new
            {
                address
            };
            
            var routine = HttpHelper.PostRequest<object>(
                $"{_apiUri}/game/start/", data);
            yield return routine;
            
            if (routine.Current == null) yield break;

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(routine.Current.ToString());
            jsonElement.TryGetProperty("response", out var response);

            if (response.ValueKind == JsonValueKind.Null) yield break;
            
            var gameSession = response.Deserialize<GameSession>();
            callback?.Invoke(gameSession);
        }

        public IEnumerator KillBoss(
            string gameId, 
            int boss)
        {
            var data = new
            {
                game_id = gameId,
                boss
            };
            var routine = HttpHelper.PostRequest<object>(
                $"{_apiUri}/game/boss/kill/", data);
            
            yield return routine;
            
        }
        
        public IEnumerator EndGameSession(
            GameResult gameResult)
        {
            var data = new
            {
                game_id = gameResult.gameId,
                score = gameResult.score,
                shots_fired = gameResult.shotsFired,
                mobs_killed = gameResult.mobsKilled
            };
            var routine = HttpHelper.PostRequest<object>(
                $"{_apiUri}/game/end/", data);
            
            yield return routine;
            yield return null;
        }
        
        public IEnumerator PauseGame(
            string gameId)
        {
            var data = new
            {
                game_id = gameId
            };
            var routine = HttpHelper.PostRequest<object>(
                $"{_apiUri}/game/pause/", data);
            
            yield return routine;
            
        }
        
        public IEnumerator ResumeGame(
            string gameId,
            Action<bool> callback)
        {
            var data = new
            {
                game_id = gameId
            };

            var routine = HttpHelper.PostRequest<object>(
                $"{_apiUri}/game/unpause/", data);
            
            yield return routine;
            
            callback?.Invoke(routine.Current != null);
        }
        
        public IEnumerator GetRewardsList(string address, Action<IEnumerable<Reward>> callback)
        {
            var routine = HttpHelper.GetRequest<string>(
                $"{_apiUri}/drop/get/?address={address}");
            yield return routine;

            if (routine.Current == null) yield break;
            var jsonResponse = JsonSerializer
                .Deserialize<JsonElement>(routine.Current.ToString());

            jsonResponse.TryGetProperty("response", out var response);
            if (response.ValueKind == JsonValueKind.Null) yield break;
            var rewards = response.Deserialize<IEnumerable<Reward>>();
            callback?.Invoke(rewards);
        }
        
        public IEnumerator GetPlayerStats(
            string address,
            Action<PlayerStats> callback)
        {
            var routine = HttpHelper.GetRequest<string>(
                $"{_apiUri}/player/stats/get/?address={address}");
            yield return routine;

            if (routine.Current == null) yield break;
            var jsonResponse = JsonSerializer
                .Deserialize<JsonElement>(routine.Current.ToString());
            
            jsonResponse.TryGetProperty("response", out var response);
            if (response.ValueKind == JsonValueKind.Null) yield break;
            
            var stats = response.Deserialize<PlayerStats>();
            
            callback?.Invoke(stats);
        }
        
        public IEnumerator ClaimReward(string address, string captcha, Action<ClaimRewardResponse> callback)
        {
            var data = new
            {
                address,
                captcha
            };

            var routine = HttpHelper.PostRequest<object>(
                $"{_apiUri}/drop/transfer/", data);
            yield return routine;

            if (routine.Current == null) yield break;

            Debug.LogError($"Response: {routine.Current.ToString()}");
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(routine.Current.ToString());
            jsonElement.TryGetProperty("response", out var response);

            if (response.ValueKind == JsonValueKind.Null) yield break;

            var claimRewardResponse = response.Deserialize<ClaimRewardResponse>();
            callback?.Invoke(claimRewardResponse);
        }
        
        public IEnumerator HasActiveSession(
            string address,
            Action<bool> callback)
        {
            var routine = HttpHelper.GetRequest<string>(
                $"{_apiUri}/player/games/has-active/?address={address}");
            yield return routine;
            
            if (routine.Current == null) yield break;
            var jsonResponse = JsonSerializer
                .Deserialize<JsonElement>(routine.Current.ToString());
            
            jsonResponse.TryGetProperty("response", out var response);
            if (response.ValueKind == JsonValueKind.Null) yield break;
            
            callback?.Invoke(response.GetProperty("has_games").GetBoolean());
        }
    }
}
