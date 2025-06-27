using UnityEngine;
using System.IO;
using Scripts.Data;
using UnityEngine.UI;
using TMPro;

namespace Scripts
{
    public class LevelLoader : MonoBehaviour
    {
        public int currentLevel;
        public LevelData levelData;

        [Header("UI")]
        public TextMeshProUGUI levelText;

        void Start()
        {
            currentLevel = LevelManager.Instance.CurrentLevel;
            LoadLevelData(currentLevel);
            levelText.text = $"Level {levelData.level_number}";
        }

        void LoadLevelData(int level)
        {
            // Load JSON file as a TextAsset from Resources
            TextAsset levelJson = Resources.Load<TextAsset>($"Levels/level_{level}");

            if (levelJson != null)
            {
                levelData = JsonUtility.FromJson<LevelData>(levelJson.text);
                Debug.Log($"Loaded level {level} with grid size {levelData.grid_width}x{levelData.grid_height}");
            }
            else
            {
                Debug.LogError($"Level file not found in Resources/Levels/level_{level}.json");
            }
        }
    }
}