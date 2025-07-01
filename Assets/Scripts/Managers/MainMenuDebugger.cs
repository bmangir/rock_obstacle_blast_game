using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class MainMenuDebugger : MonoBehaviour
    {
        [Header("Debug Info")]
        [SerializeField] private bool enableDebugMode = true;
        [SerializeField] private Button levelButton;
        [SerializeField] private TextMeshProUGUI levelButtonText;
        [SerializeField] private MainMenuController mainMenuController;
        
        private void Start()
        {
            if (!enableDebugMode) return;
            
            Debug.Log("=== MAIN MENU DEBUGGER STARTED ===");
            
            // Check if LevelManager exists
            if (LevelManager.Instance == null)
            {
                Debug.LogError("ERROR: LevelManager.Instance is NULL!");
                Debug.LogError("Make sure LevelManager GameObject exists in the scene with LevelManager script");
            }
            else
            {
                Debug.Log($"FOUND: LevelManager found! Current Level: {LevelManager.Instance.CurrentLevel}");
            }
            
            // Check if MainMenuController exists
            if (mainMenuController == null)
            {
                Debug.LogError("ERROR: MainMenuController is NULL!");
                Debug.LogError("Make sure MainMenuController GameObject exists with MainMenuController script");
            }
            else
            {
                Debug.Log("FOUND: MainMenuController found!");
            }
            
            // Check if Level Button exists
            if (levelButton == null)
            {
                Debug.LogError("ERROR: Level Button is NULL!");
                Debug.LogError("Make sure LevelButton is assigned in MainMenuController");
            }
            else
            {
                Debug.Log("FOUND Level Button found!");
                Debug.Log($"Button interactable: {levelButton.interactable}");
                Debug.Log($"Button enabled: {levelButton.enabled}");
                Debug.Log($"Button gameObject active: {levelButton.gameObject.activeInHierarchy}");
                
                // Check if button has Image component
                Image buttonImage = levelButton.GetComponent<Image>();
                if (buttonImage == null)
                {
                    Debug.LogError("ERROR: Level Button has no Image component!");
                }
                else
                {
                    Debug.Log("FOUND Level Button has Image component");
                    Debug.Log($"Image raycast target: {buttonImage.raycastTarget}");
                }
                
                // Check if button has Button component
                Button buttonComponent = levelButton.GetComponent<Button>();
                if (buttonComponent == null)
                {
                    Debug.LogError("ERROR: Level Button has no Button component!");
                }
                else
                {
                    Debug.Log("FOUND: Level Button has Button component");
                    Debug.Log($"Button interactable: {buttonComponent.interactable}");
                    Debug.Log($"Button transition: {buttonComponent.transition}");
                }
            }
            
            // Check if Level Button Text exists
            if (levelButtonText == null)
            {
                Debug.LogError("ERROR: Level Button Text is NULL!");
                Debug.LogError("Make sure LevelButtonText is assigned in MainMenuController");
            }
            else
            {
                Debug.Log("FOUND: Level Button Text found!");
                Debug.Log($"Text content: {levelButtonText.text}");
            }
            
            // Check Canvas setup
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("ERROR: No Canvas found in scene!");
            }
            else
            {
                Debug.Log("FOUND: Canvas found!");
                Debug.Log($"Canvas render mode: {canvas.renderMode}");
                Debug.Log($"Canvas enabled: {canvas.enabled}");
                
                // Check Canvas Scaler
                CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
                if (scaler == null)
                {
                    Debug.LogError("ERROR: Canvas has no CanvasScaler component!");
                }
                else
                {
                    Debug.Log("FOUND: Canvas Scaler found!");
                    Debug.Log($"UI Scale Mode: {scaler.uiScaleMode}");
                    Debug.Log($"Reference Resolution: {scaler.referenceResolution}");
                    Debug.Log($"Match: {scaler.matchWidthOrHeight}");
                }
                
                // Check Graphic Raycaster
                GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
                if (raycaster == null)
                {
                    Debug.LogError("ERROR: Canvas has no GraphicRaycaster component!");
                }
                else
                {
                    Debug.Log("FOUND: Graphic Raycaster found!");
                    Debug.Log($"Raycaster enabled: {raycaster.enabled}");
                }
            }
            
            // Check EventSystem
            UnityEngine.EventSystems.EventSystem eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                Debug.LogError("ERROR: No EventSystem found in scene!");
                Debug.LogError("Create EventSystem: Right-click in Hierarchy > UI > Event System");
            }
            else
            {
                Debug.Log("FOUND: EventSystem found!");
                Debug.Log($"EventSystem enabled: {eventSystem.enabled}");
            }
            
            Debug.Log("=== MAIN MENU DEBUGGER COMPLETED ===");
        }
        
        [ContextMenu("Test Button Click")]
        public void TestButtonClick()
        {
            if (levelButton != null)
            {
                Debug.Log("Testing button click...");
                levelButton.onClick.Invoke();
            }
            else
            {
                Debug.LogError("Cannot test button click - levelButton is null!");
            }
        }
        
        [ContextMenu("Force Load LevelScene")]
        public void ForceLoadLevelScene()
        {
            Debug.Log("Force loading LevelScene...");
            SceneManager.LoadScene("LevelScene");
        }
        
        [ContextMenu("Check Build Settings")]
        public void CheckBuildSettings()
        {
            Debug.Log("Checking build settings...");
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                Debug.Log($"Build Index {i}: {scenePath}");
            }
        }
    }
} 