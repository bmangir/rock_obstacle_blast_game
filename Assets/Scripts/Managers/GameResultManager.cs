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
                returnButton.onClick.AddListener(OnReturnClicked);
                
            if (tryAgainButton != null)
                tryAgainButton.onClick.AddListener(OnTryAgainClicked);
                
            if (loseReturnButton != null)
                loseReturnButton.onClick.AddListener(OnReturnClicked);
        }
        
        public void ShowWinScreen()
        {
            if (winScreen is not null)
            {
                winScreen.SetActive(true);
                
                // Check if this is the last level (level 10)
                if (LevelManager.Instance.IsLastLevel())
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
                        winLevelText.text = $"Level {LevelManager.Instance.CurrentLevel} Complete!";
                        
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
            int nextLevel = LevelManager.Instance.CurrentLevel + 1;
            LevelManager.Instance.SetCurrentLevel(nextLevel);
            
            // If all levels are completed (level 10 completed),
            // go to MainScene to show "All Finished"
            if (nextLevel > LevelManager.Instance.MaxLevel)
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
        
        private void OnReturnClicked()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
} 