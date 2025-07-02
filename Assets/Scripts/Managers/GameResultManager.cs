using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameResultManager : MonoBehaviour
    {
        [Header("Win Screen")]
        [SerializeField] private GameObject winScreen;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button returnButton;
        [SerializeField] private TextMeshProUGUI winLevelText;
        
        [Header("Lose Screen")]
        [SerializeField] private GameObject loseScreen;
        [SerializeField] private Button tryAgainButton;
        [SerializeField] private Button loseReturnButton;
        [SerializeField] private TextMeshProUGUI gameOverText;
        
        [Header("UI References")]
        [SerializeField] private Canvas resultCanvas;
        
        private void Start()
        {
            SetupButtons();
            HideAllScreens();
        }
        
        private void SetupButtons()
        {
            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinueClicked);
                
            if (returnButton != null)
                returnButton.onClick.AddListener(OnWinReturnClicked);
                
            if (tryAgainButton != null)
                tryAgainButton.onClick.AddListener(OnTryAgainClicked);
                
            if (loseReturnButton != null)
                loseReturnButton.onClick.AddListener(OnLoseReturnClicked);
        }
        
        public void ShowWinScreen()
        {
            // Show the completed level
            int completedLevel = LevelManager.Instance.CurrentLevel;
            bool isLastLevel = LevelManager.Instance.IsLastLevel();
            
            if (winScreen is not null)
            {
                winScreen.SetActive(true);
                
                // Check if this was the last level (level 10)
                if (isLastLevel)
                {
                    if (winLevelText is not null)
                        winLevelText.text = "You Won All Levels !";
                        
                    // Hide continue button for final level, only show return
                    if (continueButton is not null)
                        continueButton.gameObject.SetActive(false);
                        
                    if (returnButton is not null)
                        returnButton.gameObject.SetActive(true);
                }
                else
                {
                    if (winLevelText is not null)
                        winLevelText.text = $"Level {completedLevel} Complete!";
                        
                    // Show both buttons for normal levels
                    if (continueButton is not null)
                        continueButton.gameObject.SetActive(true);
                        
                    if (returnButton is not null)
                        returnButton.gameObject.SetActive(true);
                }
            }
            
            if (resultCanvas is not null)
                resultCanvas.sortingOrder = 100;
        }
        
        public void ShowLoseScreen()
        {
            if (loseScreen is not null)
            {
                loseScreen.SetActive(true);
                if (gameOverText is not null)
                    gameOverText.text = "Game Over!";
            }
            
            if (resultCanvas is not null)
                resultCanvas.sortingOrder = 100;
        }
        
        private void HideAllScreens()
        {
            if (winScreen != null) winScreen.SetActive(false);
            if (loseScreen != null) loseScreen.SetActive(false);
        }
        
        private void OnContinueClicked()
        {
            // Increment level when user clicks Continue
            if (!LevelManager.Instance.IsLastLevel())
            {
                LevelManager.Instance.SetCurrentLevel(LevelManager.Instance.CurrentLevel + 1);
            }
            
            // Navigate to next level or main scene if all levels completed
            if (LevelManager.Instance.AllLevelsFinished())
            {
                SceneManager.LoadScene("MainScene");
            }
            else
            {
                SceneManager.LoadScene("LevelScene");
            }
        }
        
        private void OnTryAgainClicked()
        {
            SceneManager.LoadScene("LevelScene");
        }
        
        private void OnWinReturnClicked()
        {
            // Increment level when user clicks Return from WIN screen
            // Update the Level Button text in MainScene
            if (!LevelManager.Instance.IsLastLevel())
            {
                LevelManager.Instance.SetCurrentLevel(LevelManager.Instance.CurrentLevel + 1);
            }
            
            SceneManager.LoadScene("MainScene");
        }
        
        private void OnLoseReturnClicked()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
} 