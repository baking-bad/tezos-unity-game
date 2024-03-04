using Managers;
using UnityEngine;

namespace UI
{
    public class PauseCountdown : MonoBehaviour
    {
        private LevelManager _levelManager;
        private void Awake()
        {
            _levelManager = GameObject.FindGameObjectWithTag("GameController")
                .GetComponent<LevelManager>();
        }

        public void Resume()
        {
            _levelManager.Resume();
        }
    }
}
