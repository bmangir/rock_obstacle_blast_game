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
            levelButton.interactable = true;
            if (LevelManager.Instance.AllLevelsFinished())
            {
                levelButtonText.text = "All Finished";
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