using System;
using Api.Models;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class StatsCreator : MonoBehaviour
    {
        [SerializeField] private TMP_Text gamesPlayed;
        [SerializeField] private TMP_Text bestScore;
        [SerializeField] private TMP_Text bossesKilled;
        [SerializeField] private TMP_Text mobsKilled;
        [SerializeField] private TMP_Text shotsFired;
        [SerializeField] private TMP_Text favouriteWeapon;

        void Start()
        {
            RenderStats(UserDataManager.Instance.GetPlayerStats());
        }

        private void RenderStats(PlayerStats stats)
        {
            gamesPlayed.text = stats.GamesPlayed.ToString();
            bestScore.text = stats.BestScore.ToString();
            bossesKilled.text = stats.BossesKilled.ToString();
            mobsKilled.text = stats.MobsKilled.ToString();
            shotsFired.text = stats.ShotsFired.ToString();
        }
    }
}
