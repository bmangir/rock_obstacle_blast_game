using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Blocks;
using UnityEngine.SceneManagement;

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
        [SerializeField] private TextMeshProUGUI movesText;
        [SerializeField] private List<GoalItem> goalItems = new List<GoalItem>();
        
        private Dictionary<ObstacleType, int> goalCounts = new Dictionary<ObstacleType, int>();
        private int remainingMoves;

        public void Initialize(Dictionary<ObstacleType, int> goals, int moveCount)
        {
            goalCounts = goals;
            remainingMoves = moveCount;
            UpdateMovesDisplay();

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
            UpdateMovesDisplay();
            CheckWinLose();
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
                        item.icon.gameObject.SetActive(false);
                        item.checkMark.SetActive(true);
                    }
                    break;
                }
            }
            
            CheckWinLose();
        }

        public int GetRemainingObstacles()
        {
            int count = 0;
            foreach (var goal in goalCounts)
            {
                count += goal.Value;
            }
            return count;
        }

        private void UpdateMovesDisplay()
        {
            movesText.text = remainingMoves.ToString();
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

            // Only win if ALL obstacles are cleared
            if (allGoalsComplete && GetRemainingObstacles() == 0)
            {
                LevelManager.Instance.LoadNextLevel();
            }
            else if (remainingMoves <= 0)
            {
                LevelManager.Instance.ReloadCurrentLevel();
            }
        }
    }
}