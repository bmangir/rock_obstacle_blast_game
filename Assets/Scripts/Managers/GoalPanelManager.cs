using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Blocks;

namespace Managers
{
    public class GoalPanelManager : MonoBehaviour
    {
        [System.Serializable]
        public class GoalItem
        {
            public ObstacleType type;
            public Image icon;
            public TextMeshProUGUI countText;
            public GameObject checkMark;
        }

        [Header("References")]
        [SerializeField] private TextMeshProUGUI movesTitle;
        [SerializeField] private TextMeshProUGUI movesCount;
        [SerializeField] private List<GoalItem> goalItems = new List<GoalItem>();
        
        private Dictionary<ObstacleType, int> goalCounts = new Dictionary<ObstacleType, int>();
        private int remainingMoves;
        private int totalMoves; // Track total moves for star calculation
        private int usedMoves; // Track used moves for star calculation

        public int GetUsedMoves() => usedMoves;
        public int GetTotalMoves() => totalMoves;

        public void Initialize(Dictionary<ObstacleType, int> goals, int moveCount)
        {
            goalCounts = goals;
            remainingMoves = moveCount;
            totalMoves = moveCount;
            usedMoves = 0;
            UpdateMovesDisplay();

            // Start timer when level begins
            if (StarRatingManager.Instance != null)
            {
                StarRatingManager.Instance.StartTimer();
            }

            // Activate only needed goal items
            foreach (var item in goalItems)
            {
                bool hasGoal = goalCounts.ContainsKey(item.type) && goalCounts[item.type] > 0;
                item.icon.gameObject.SetActive(hasGoal);
                item.countText.gameObject.SetActive(hasGoal);
                item.checkMark.SetActive(false);
                
                if (hasGoal)
                {
                    item.countText.text = goalCounts[item.type].ToString();
                }
            }
        }

        public void DecrementMove()
        {
            if (remainingMoves <= 0) return;
            
            remainingMoves--;
            usedMoves++;
            
            UpdateMovesDisplay();
        }

        public void DecrementObstacle(ObstacleType type)
        {
            if (!goalCounts.ContainsKey(type) || goalCounts[type] <= 0) return;
            
            goalCounts[type]--;
            
            foreach (var item in goalItems)
            {
                if (item.type == type)
                {
                    item.countText.text = goalCounts[type].ToString();
                    
                    if (goalCounts[type] == 0)
                    {
                        item.countText.gameObject.SetActive(false);
                        item.checkMark.SetActive(true);
                    }
                    break;
                }
            }
            
            CheckWinLose();
        }

        private void UpdateMovesDisplay()
        {
            if (movesCount is not null)
                movesCount.text = remainingMoves.ToString();
        }

        private void CheckWinLose()
        {
            bool allGoalsComplete = true;
            foreach (var goal in goalCounts)
            {
                if (goal.Value > 0)
                {
                    allGoalsComplete = false;
                    break;
                }
            }

            // PRIORITY 1: Win condition - if all obstacles are cleared, player wins
            // This takes precedence over move count (even if remaining moves = 0)
            // Player can win the level with his/her last move
            if (allGoalsComplete)
            {
                // Stop timer when level is won
                if (StarRatingManager.Instance != null)
                {
                    StarRatingManager.Instance.StopTimer();
                }
                StartCoroutine(DelayedWinScreen());
            }
            // PRIORITY 2: Lose condition - only if goals are not complete and no moves left
            else if (remainingMoves <= 0)
            {
                if (StarRatingManager.Instance != null)
                {
                    StarRatingManager.Instance.StopTimer();
                }
                StartCoroutine(DelayedLoseScreen());
            }
        }
        
        private System.Collections.IEnumerator DelayedWinScreen()
        {
            // Wait for all animations to complete
            yield return new WaitForSeconds(1.5f);
            
            GameResultManager resultManager = FindObjectOfType<GameResultManager>();
            if (resultManager is not null)
            {
                resultManager.ShowWinScreen();
            }
        }
        
        private System.Collections.IEnumerator DelayedLoseScreen()
        {
            // Wait for all animations to complete
            yield return new WaitForSeconds(1.5f);
            
            GameResultManager resultManager = FindObjectOfType<GameResultManager>();
            if (resultManager is not null)
            {
                resultManager.ShowLoseScreen();
            }
        }
        
        public void CheckWinLoseAfterMove()
        {
            CheckWinLose();
        }
    }
}