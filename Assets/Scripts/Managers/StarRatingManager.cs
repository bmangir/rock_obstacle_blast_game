using UnityEngine;
using Data;

namespace Managers
{
    public class StarRatingManager : MonoBehaviour
    {
        public static StarRatingManager Instance { get; private set; }
        
        [Header("Star Rating Settings")]
        [SerializeField] private float excellentTimeThreshold = 30f; // seconds
        [SerializeField] private float goodTimeThreshold = 60f; // seconds
        
        private float levelStartTime;
        private bool isTimerActive;
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void StartTimer()
        {
            levelStartTime = Time.time;
            isTimerActive = true;
        }
        
        public void StopTimer()
        {
            isTimerActive = false;
        }
        
        public float GetElapsedTime()
        {
            if (!isTimerActive) return 0f;
            return Time.time - levelStartTime;
        }
        
        public int CalculateStarRating(LevelData levelData, int usedMoves, float completionTime)
        {
            // Calculate optimal moves if not provided in level data
            int optimalMoves = levelData.optimal_moves > 0 ? 
                levelData.optimal_moves : 
                CalculateOptimalMoves(levelData);
            
            // Move efficiency: how close to optimal moves
            float moveEfficiency = (float)optimalMoves / usedMoves;
            moveEfficiency = Mathf.Clamp01(moveEfficiency);
            
            // Time efficiency: how fast the level was completed
            float timeEfficiency = CalculateTimeEfficiency(completionTime);
            
            // Combined score (60% moves, 40% time)
            float totalScore = (moveEfficiency * 0.6f) + (timeEfficiency * 0.4f);
            
            // Determine star rating
            if (totalScore >= 0.85f) return 3; // 3 stars: 85%+ efficiency
            if (totalScore >= 0.65f) return 2; // 2 stars: 65%+ efficiency
            return 1; // 1 star: completed but below 65%
        }
        
        private int CalculateOptimalMoves(LevelData levelData)
        {
            // Count total obstacles to estimate optimal moves
            int totalObstacles = 0;
            foreach (string cell in levelData.grid)
            {
                if (cell == "bo" || cell == "s" || cell == "v")
                {
                    totalObstacles++;
                }
            }
            
            // Estimate optimal moves based on level complexity
            // Formula: base moves + obstacle factor + level number factor
            int baseMoves = Mathf.Max(3, totalObstacles / 3); // At least 3 moves
            int levelFactor = Mathf.Max(1, levelData.level_number / 2); // Harder levels need more moves
            
            int estimated = baseMoves + levelFactor;
            
            // Cap at 70% of given move count to ensure achievability
            int maxOptimal = Mathf.RoundToInt(levelData.move_count * 0.7f);
            
            return Mathf.Min(estimated, maxOptimal);
        }
        
        private float CalculateTimeEfficiency(float completionTime)
        {
            if (completionTime <= excellentTimeThreshold)
                return 1.0f; // Perfect time score
            
            if (completionTime <= goodTimeThreshold)
            {
                // Linear interpolation between excellent and good thresholds
                float ratio = (completionTime - excellentTimeThreshold) / 
                             (goodTimeThreshold - excellentTimeThreshold);
                return Mathf.Lerp(1.0f, 0.7f, ratio);
            }
            
            // Beyond good threshold, diminishing returns
            float overtime = completionTime - goodTimeThreshold;
            float timeScore = 0.7f - (overtime * 0.01f); // -1% per second over threshold
            
            return Mathf.Clamp01(timeScore);
        }
        
        public string GetStarDescription(int stars)
        {
            return stars switch
            {
                3 => "Perfect!",
                2 => "Great!",
                1 => "Good!",
                _ => "Try Again"
            };
        }
        
        public Color GetStarColor(int stars)
        {
            return stars switch
            {
                3 => new Color(1f, 0.84f, 0f, 1f),
                2 => new Color(0.9f, 0.9f, 0.9f, 1f),  
                1 => new Color(0.8f, 0.52f, 0.25f, 1f),
                _ => new Color(0.4f, 0.4f, 0.4f, 0.8f)
            };
        }
    }
} 