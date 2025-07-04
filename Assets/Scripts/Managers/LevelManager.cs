using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        private const string LastPlayedLevelKey = "LastPlayedLevel";
        public int CurrentLevel { get; private set; }
        public int MaxLevel = 10;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadCurrentLevel();
            }
            else
            {
                Destroy(gameObject); // Avoid duplicates
            }
        }

        private void LoadCurrentLevel()
        {
            CurrentLevel = PlayerPrefs.GetInt(LastPlayedLevelKey, 1);
            Debug.Log($"Current level: {CurrentLevel}");
        }

        public void SetCurrentLevel(int level)
        {
            CurrentLevel = level;
            PlayerPrefs.SetInt(LastPlayedLevelKey, CurrentLevel);
            PlayerPrefs.Save();
        }

        public bool AllLevelsFinished()
        {
            return CurrentLevel > MaxLevel;
        }
        
        public bool IsLastLevel()
        {
            return CurrentLevel == MaxLevel;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("BlastGame/Set Last Played Level/Level 1")]
        public static void SetLevel1() => SetLevelFromEditor(1);

        [UnityEditor.MenuItem("BlastGame/Set Last Played Level/Level 10")]
        public static void SetLevel10() => SetLevelFromEditor(10);
        
        [UnityEditor.MenuItem("BlastGame/Set Last Played Level/All Levels Complete")]
        public static void SetAllComplete() => SetLevelFromEditor(11);

        private static void SetLevelFromEditor(int level)
        {
            PlayerPrefs.SetInt(LastPlayedLevelKey, level);
            PlayerPrefs.Save();
            Debug.Log($"Set LastPlayedLevel to {level}");
            
            if (Instance != null)
            {
                Instance.LoadCurrentLevel();
            }
        }
#endif
    }
}