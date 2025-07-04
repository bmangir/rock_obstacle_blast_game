using UnityEngine;
using Blocks;

namespace Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }
        
        [Header("Score Settings")]
        [SerializeField] private int cubeScore = 10;
        [SerializeField] private int obstacleBoxScore = 50;
        [SerializeField] private int obstacleStoneScore = 75;
        [SerializeField] private int obstacleVaseScore = 100;
        [SerializeField] private int rocketScore = 200;
        [SerializeField] private int remainingRocketBonus = 500;
        [SerializeField] private int remainingMoveBonus = 25;
        
        [Header("Star Multipliers")]
        [SerializeField] private float oneStarMultiplier = 1.0f;
        [SerializeField] private float twoStarMultiplier = 1.5f;
        [SerializeField] private float threeStarMultiplier = 2.0f;
        
        private int currentScore = 0;
        private int cubesDestroyed = 0;
        private int obstaclesDestroyed = 0;
        private int rocketsUsed = 0;
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void ResetScore()
        {
            currentScore = 0;
            cubesDestroyed = 0;
            obstaclesDestroyed = 0;
            rocketsUsed = 0;
        }
        
        public void AddCubeScore(int count = 1)
        {
            cubesDestroyed += count;
            currentScore += cubeScore * count;
            Debug.Log($"Cubes destroyed: +{count}, Score: +{cubeScore * count}");
        }
        
        public void AddObstacleScore(ObstacleType obstacleType)
        {
            obstaclesDestroyed++;
            int points = obstacleType switch
            {
                ObstacleType.Box => obstacleBoxScore,
                ObstacleType.Stone => obstacleStoneScore,
                ObstacleType.Vase => obstacleVaseScore,
                _ => obstacleBoxScore
            };
            
            currentScore += points;
            Debug.Log($"Obstacle destroyed ({obstacleType}): +{points} points");
        }
        
        public void AddRocketScore()
        {
            rocketsUsed++;
            currentScore += rocketScore;
            Debug.Log($"Rocket used: +{rocketScore} points");
        }
        
        public int CalculateFinalScore(int starRating, int remainingMoves, int remainingRockets)
        {
            int finalScore = currentScore;
            
            // Add remaining move bonus
            int moveBonus = remainingMoves * remainingMoveBonus;
            finalScore += moveBonus;
            
            // Add remaining rocket bonus
            int rocketBonus = remainingRockets * remainingRocketBonus;
            finalScore += rocketBonus;
            
            // Apply star multiplier
            float multiplier = starRating switch
            {
                3 => threeStarMultiplier,
                2 => twoStarMultiplier,
                1 => oneStarMultiplier,
                _ => 1.0f
            };
            
            finalScore = Mathf.RoundToInt(finalScore * multiplier);
            
            return finalScore;
        }
        
        public string GetScoreBreakdown(int starRating, int remainingMoves, int remainingRockets)
        {
            int moveBonus = remainingMoves * remainingMoveBonus;
            int rocketBonus = remainingRockets * remainingRocketBonus;
            
            string breakdown = $"Base Score: {currentScore}\n";
            
            if (moveBonus > 0)
                breakdown += $"Move Bonus: +{moveBonus}\n";
                
            if (rocketBonus > 0)
                breakdown += $"Rocket Bonus: +{rocketBonus}\n";
                
            float multiplier = starRating switch
            {
                3 => threeStarMultiplier,
                2 => twoStarMultiplier,
                1 => oneStarMultiplier,
                _ => 1.0f
            };
            
            if (multiplier > 1.0f)
                breakdown += $"Star Bonus: x{multiplier}";
            
            return breakdown;
        }
        
        public int GetCurrentScore() => currentScore;
        public int GetCubesDestroyed() => cubesDestroyed;
        public int GetObstaclesDestroyed() => obstaclesDestroyed;
        public int GetRocketsUsed() => rocketsUsed;
    }
} 