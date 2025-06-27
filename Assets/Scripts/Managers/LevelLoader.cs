using UnityEngine;
using Data;

namespace Managers
{
    public class LevelLoader : MonoBehaviour
    {
        public LevelData levelData;

        void Awake()
        {
            LoadLevel(LevelManager.Instance.CurrentLevel);
        }

        public void LoadLevel(int level)
        {
            string path = $"Levels/level_{level:D2}";
            TextAsset json = Resources.Load<TextAsset>(path);
            if (json != null)
                levelData = JsonUtility.FromJson<LevelData>(json.text);
            else
                Debug.LogError($"Level file not found at: {path}");
        }
    }
}