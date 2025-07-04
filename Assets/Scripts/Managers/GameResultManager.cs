using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Data;

namespace Managers
{
    public class GameResultManager : MonoBehaviour
    {
        [Header("Win Screen")]
        [SerializeField] private GameObject winScreen;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button returnButton;
        [SerializeField] private TextMeshProUGUI winLevelText;
        
        [Header("Star Rating UI")]
        [SerializeField] private GameObject starContainer;
        [SerializeField] private Image[] starImages = new Image[3]; // 3 stars max
        [SerializeField] private TextMeshProUGUI starDescriptionText;
        
        [Header("Lose Screen")]
        [SerializeField] private GameObject loseScreen;
        [SerializeField] private Button tryAgainButton;
        [SerializeField] private Button loseReturnButton;
        [SerializeField] private TextMeshProUGUI gameOverText;
        [SerializeField] private GameObject buttonWrapper;
        
        [Header("UI References")]
        [SerializeField] private Canvas resultCanvas;
        
        [Header("Star Graphics")]
        [SerializeField] private Sprite starFilledSprite;
        [SerializeField] private Sprite starEmptySprite;
        
        private void Start()
        {
            SetupButtons();
            HideAllScreens();
            LoadStarSprites();
        }
        
        private void LoadStarSprites()
        {
            // Load star sprites from resources if not assigned
            if (starFilledSprite == null)
            {
                starFilledSprite = Resources.Load<Sprite>("UI/Gameplay/Celebration/star");
            }
            
            if (starEmptySprite == null)
            {
                // Create a grayed out version or use same sprite with different color
                starEmptySprite = starFilledSprite;
            }
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
            
            // Calculate star rating
            CalculateAndDisplayStars(completedLevel);
            
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
            
            if (buttonWrapper != null)
            {
                buttonWrapper.SetActive(!isLastLevel);
            }
            
            if (resultCanvas is not null)
                resultCanvas.sortingOrder = 100;
        }
        
        private void CalculateAndDisplayStars(int levelNumber)
        {
            if (StarRatingManager.Instance == null) return;
            
            // Get level data
            LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
            if (levelLoader == null || levelLoader.levelData == null) return;
            
            // Get performance data
            GoalPanelManager goalManager = FindObjectOfType<GoalPanelManager>();
            if (goalManager == null) return;
            
            int usedMoves = goalManager.GetUsedMoves();
            float completionTime = StarRatingManager.Instance.GetElapsedTime();
            
            // Calculate star rating
            int stars = StarRatingManager.Instance.CalculateStarRating(
                levelLoader.levelData, usedMoves, completionTime);
            
            DisplayStarRating(stars, usedMoves, completionTime);
            
            Debug.Log($"Level {levelNumber} completed with {stars} stars! " +
                     $"Used {usedMoves} moves in {completionTime:F1} seconds");
        }
        
        private void DisplayStarRating(int earnedStars, int usedMoves, float completionTime)
        {
            if (starContainer != null)
                starContainer.SetActive(true);
            
            // Update star visuals
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] != null)
                {
                    // Always use the same star sprite
                    starImages[i].sprite = starFilledSprite;
                    
                    if (i < earnedStars)
                    {
                        starImages[i].color = StarRatingManager.Instance.GetStarColor(earnedStars);
                    }
                    else
                    {
                        starImages[i].color = new Color(0.4f, 0.4f, 0.4f, 0.8f);
                    }
                    starImages[i].gameObject.SetActive(true);
                }
            }
            
            if (starDescriptionText != null)
            {
                starDescriptionText.text = StarRatingManager.Instance.GetStarDescription(earnedStars);
            }
            
            if (earnedStars > 0)
            {
                StartCoroutine(CreateStarCelebrationEffect(earnedStars));
            }
        }
        
        public void ShowLoseScreen()
        {
            if (loseScreen is not null)
            {
                loseScreen.SetActive(true);
                if (gameOverText is not null)
                    gameOverText.text = "Game Over!";
                    
                if (buttonWrapper != null)
                {
                    bool isLastLevel = LevelManager.Instance.IsLastLevel();
                    buttonWrapper.SetActive(!isLastLevel);
                }
            }
            
            if (resultCanvas is not null)
                resultCanvas.sortingOrder = 100;
        }
        
        private void HideAllScreens()
        {
            if (winScreen != null) winScreen.SetActive(false);
            if (loseScreen != null) loseScreen.SetActive(false);
            if (starContainer != null) starContainer.SetActive(false);
        }
        
        private void OnContinueClicked()
        {
            LevelManager.Instance.SetCurrentLevel(LevelManager.Instance.CurrentLevel + 1);
            
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
            LevelManager.Instance.SetCurrentLevel(LevelManager.Instance.CurrentLevel + 1);
            
            SceneManager.LoadScene("MainScene");
        }
        
        private void OnLoseReturnClicked()
        {
            SceneManager.LoadScene("MainScene");
        }
        
        private System.Collections.IEnumerator CreateStarCelebrationEffect(int earnedStars)
        {
            // Wait for UI to settle
            yield return new WaitForSeconds(0.3f);
            
            // Create star burst from each earned star
            for (int i = 0; i < earnedStars; i++)
            {
                if (starImages[i] != null)
                {
                    Vector3 starWorldPos = starImages[i].transform.position;
                    
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(
                        starWorldPos.x, starWorldPos.y, 10f));
                    
                    CreateStarBurstEffect(worldPos, earnedStars);
                    
                    StartCoroutine(AnimateStarReveal(starImages[i].transform, i * 0.2f));
                }
                
                yield return new WaitForSeconds(0.3f); // Stagger the star effects
            }
            
            if (starContainer != null)
            {
                Vector3 centerPos = starContainer.transform.position;
                Vector3 worldCenterPos = Camera.main.ScreenToWorldPoint(new Vector3(
                    centerPos.x, centerPos.y, 10f));
                
                CreateGrandCelebrationBurst(worldCenterPos, earnedStars);
            }
        }
        
        private void CreateStarBurstEffect(Vector3 position, int earnedStars)
        {
            if (ParticleEffectManager.Instance != null)
            {
                Color starColor = StarRatingManager.Instance.GetStarColor(earnedStars);
                
                for (int i = 0; i < 8; i++)
                {
                    float angle = i * 45f * Mathf.Deg2Rad;
                    Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                    Vector3 spawnPos = position + direction * 0.2f;
                    
                    ParticleEffectManager.Instance.CreateCubeBlastEffect(spawnPos, starColor, "star");
                }
            }
        }
        
        private void CreateGrandCelebrationBurst(Vector3 position, int earnedStars)
        {
            if (ParticleEffectManager.Instance != null)
            {
                Color starColor = StarRatingManager.Instance.GetStarColor(earnedStars);
                
                // Create a spectacular burst based on star count
                int burstCount = earnedStars * 12;
                
                for (int i = 0; i < burstCount; i++)
                {
                    float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                    float distance = Random.Range(0.5f, 2f);
                    Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                    Vector3 spawnPos = position + direction * distance;
                    
                    ParticleEffectManager.Instance.CreateCubeBlastEffect(spawnPos, starColor, "star");
                }
            }
        }
        
        private System.Collections.IEnumerator AnimateStarReveal(Transform starTransform, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            Vector3 originalScale = starTransform.localScale;
            starTransform.localScale = Vector3.zero;
            
            // Scale up with bounce effect
            float duration = 0.6f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = elapsed / duration;
                
                // Elastic ease out for bouncy effect
                float scale = ElasticEaseOut(progress);
                starTransform.localScale = originalScale * scale;
                
                yield return null;
            }
            
            starTransform.localScale = originalScale;
        }
        
        private float ElasticEaseOut(float t)
        {
            if (t == 0f) return 0f;
            if (t == 1f) return 1f;
            
            float p = 0.3f;
            float s = p / 4f;
            
            return (Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - s) * (2f * Mathf.PI) / p) + 1f);
        }
    }
} 