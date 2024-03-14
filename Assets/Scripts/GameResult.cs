using UnityEngine;

public class GameResult : MonoBehaviour
{
    [SerializeField]
    public string gameId;
    [SerializeField]
    public int score;
    [SerializeField]
    public int mobsKilled;
    [SerializeField]
    public string favoriteWeapon;
    [SerializeField]
    public int shotsFired;

    public void Init(string gameId)
    {
        this.gameId = gameId;
        score = 0;
    }

    public int GetScore() => score;

    public void UpdateScore()
    {
        // todo: dict with enemy score
        score++;
        mobsKilled++;
    }
    
    public void UpdateShotsFired()
    {
        shotsFired++;
    }
}
