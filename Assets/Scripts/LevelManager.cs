// Scripts/LevelManager.cs
using UnityEngine;

namespace Scripts
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        public int CurrentLevel { get; private set; } = 1; // Default level

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                LoadLevelData();
            }
        }

        void LoadLevelData()
        {
            // Load current level from PlayerPrefs or set to 1
            CurrentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        }

        public void AdvanceLevel()
        {
            CurrentLevel++;
            PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
        }
    }

}