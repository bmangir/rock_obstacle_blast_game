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
            }
            else
            {
                Destroy(gameObject); // Avoid duplicates
            }

            LoadCurrentLevel();
        }

        private void LoadCurrentLevel()
        {
            CurrentLevel = PlayerPrefs.GetInt(LastPlayedLevelKey, 1);
        }

        public void SetCurrentLevel(int level)
        {
            CurrentLevel = Mathf.Clamp(level, 1, MaxLevel);
            PlayerPrefs.SetInt(LastPlayedLevelKey, CurrentLevel);
            PlayerPrefs.Save();
        }

        public bool AllLevelsFinished()
        {
            return CurrentLevel > MaxLevel;
        }
        
        // TODO: The next 2 functions will be implement in the ResultScene and Manager
        public void LoadNextLevel()
        {
            CurrentLevel = Mathf.Clamp(CurrentLevel + 1, 1, MaxLevel);
            PlayerPrefs.SetInt(LastPlayedLevelKey, CurrentLevel);
            PlayerPrefs.Save();
            SceneManager.LoadScene("LevelScene"); // TODO: Load ResultScene later
        }

        public void ReloadCurrentLevel()
        {
            SceneManager.LoadScene("LevelScene"); // TODO: Load ResultScene latter
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("BlastGame/Set Last Played Level/Level 1")]
        public static void SetLevel1() => SetLevelFromEditor(1);

        [UnityEditor.MenuItem("BlastGame/Set Last Played Level/Level 10")]
        public static void SetLevel10() => SetLevelFromEditor(10);

        private static void SetLevelFromEditor(int level)
        {
            PlayerPrefs.SetInt(LastPlayedLevelKey, level);
            PlayerPrefs.Save();
            Debug.Log($"Set LastPlayedLevel to {level}");
        }
#endif
    }
}