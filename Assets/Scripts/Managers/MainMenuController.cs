using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelButtonText;
        [SerializeField] private Button levelButton;

        private void Start()
        {
            UpdateLevelButton();
        }
        
        private void OnEnable()
        {
            UpdateLevelButton();
        }

        private void UpdateLevelButton()
        {
            if (LevelManager.Instance == null)
            {
                Invoke(nameof(UpdateLevelButton), 0.1f);
                return;
            }
            
            // Clear any existing listeners to prevent duplicates
            levelButton.onClick.RemoveAllListeners();
            
            if (LevelManager.Instance.AllLevelsFinished())
            {
                levelButtonText.text = "Finished!";
                levelButton.interactable = false;
            }
            else
            {
                levelButtonText.text = $"Level {LevelManager.Instance.CurrentLevel}";
                levelButton.interactable = true;
                levelButton.onClick.AddListener(OnLevelButtonClicked);
            }
        }

        private void OnLevelButtonClicked()
        {
            SceneManager.LoadScene("LevelScene", LoadSceneMode.Single);
        }
    }
}