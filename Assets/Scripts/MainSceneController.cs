namespace Scripts
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using UnityEngine.SceneManagement;

    public class MainSceneController : MonoBehaviour
    {
        public Button levelButton;
        public TextMeshProUGUI levelText;

        void Start()
        {
            int currentLevel = LevelManager.Instance.CurrentLevel;
            levelText.text = $"Level {currentLevel}";
            levelButton.onClick.AddListener(() => StartLevel(currentLevel));
        }

        void StartLevel(int level)
        {
            // Load the game scene and pass the level number if needed
            SceneManager.LoadScene("GameScene");
        }
    }
}