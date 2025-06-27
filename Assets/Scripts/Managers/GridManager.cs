using UnityEngine;
using System.Collections.Generic;
using Blocks;
using Data;
using Managers;

namespace Managers
{
    public class GridManager : MonoBehaviour
    {
        public GameObject cubePrefab;
        [SerializeField] private GameObject obstaclePrefab;
        [SerializeField] private GameObject rocketPrefab;
        
        public LevelLoader levelLoader;

        private int width;
        private int height;
        private Transform gridRoot;
        private CubeBlock[,] grid;
        
        [SerializeField] private float blockSize = 1.2f;
        [SerializeField] private Vector2 backgroundPadding = new Vector2(1f, 1f);

        void Start()
        {
            InitGrid();
            AdjustCameraToFitGrid();
        }
        
        void AdjustCameraToFitGrid()
        {
            Camera cam = Camera.main;

            float verticalFit = (height + 1) * blockSize / 2f;
            float horizontalFit = (width + 1) * blockSize / 2f / cam.aspect;

            cam.orthographicSize = Mathf.Max(verticalFit, horizontalFit);
            cam.transform.position = new Vector3(0, 0, -10);
        }


        void InitGrid()
        {
            LevelData data = levelLoader.levelData;
            width = data.grid_width;
            height = data.grid_height;

            gridRoot = new GameObject("GridRoot").transform;
            grid = new CubeBlock[width, height];

            // Calculate grid offset for centering
            Vector3 gridOffset = new Vector3(-(width - 1) * blockSize / 2f, -(height - 1) * blockSize / 2f, 0);

            for (int i = 0; i < data.grid.Count; i++)
            {
                int x = i % width;
                int y = i / width;

                Vector2Int pos = new Vector2Int(x, y);
                Vector3 worldPos = new Vector3(x * blockSize, y * blockSize, 0) + gridOffset;

                string type = data.grid[i] == "rand" ? GetRandomColorCode() : data.grid[i];

                if (type == "bo" || type == "s" || type == "v")
                {
                    CreateObstacle(type, pos, worldPos);
                }
                else if (type == "hro" || type == "vro")
                {
                    CreateRocket(type, pos, worldPos);
                }
                else
                {
                    BlockColor color = ParseColor(type);
                    CreateCube(color, pos, worldPos);
                }
            }
            
            CreateGridBackground();
        }
        
        void CreateGridBackground()
        {
            Sprite bgSprite = Resources.Load<Sprite>("UI/Gameplay/grid_background");
            if (bgSprite == null)
            {
                Debug.LogError("Grid background sprite not found at Resources/UI/Gameplay/grid_background.png");
                return;
            }

            GameObject bgObj = new GameObject("GridBackground");
            SpriteRenderer sr = bgObj.AddComponent<SpriteRenderer>();
            sr.sprite = bgSprite;
            sr.sortingOrder = -5;

            float widthWorld = width * blockSize + backgroundPadding.x;
            float heightWorld = height * blockSize + backgroundPadding.y;
            bgObj.transform.localScale = new Vector3(
                widthWorld / sr.sprite.bounds.size.x,
                heightWorld / sr.sprite.bounds.size.y,
                1f
            );

            bgObj.transform.position = new Vector3(0, 0, 1); // behind blocks, in front of background
        }


        void CreateCube(BlockColor color, Vector2Int gridPos, Vector3 worldPos)
        {
            GameObject obj = Instantiate(cubePrefab, worldPos, Quaternion.identity, gridRoot);
            CubeBlock cube = obj.GetComponent<CubeBlock>();
            cube.Initialize(color, gridPos);
            grid[gridPos.x, gridPos.y] = cube;
        }
        
        private void CreateObstacle(string typeCode, Vector2Int gridPos, Vector3 worldPos)
        {
            GameObject obj = Instantiate(obstaclePrefab, worldPos, Quaternion.identity, gridRoot);
            ObstacleBlock block = obj.GetComponent<ObstacleBlock>();

            if (block == null)
            {
                Debug.LogError("Missing ObstacleBlock script on prefab");
                return;
            }

            ObstacleType obstacleType = typeCode switch
            {
                "bo" => ObstacleType.Box,
                "s" => ObstacleType.Stone,
                "v" => ObstacleType.Vase,
                _ => ObstacleType.Box
            };

            block.Initialize(obstacleType);
        }
        
        private void CreateRocket(string typeCode, Vector2Int gridPos, Vector3 worldPos)
        {
            GameObject obj = Instantiate(rocketPrefab, worldPos, Quaternion.identity, gridRoot);
            RocketBlock rocket = obj.GetComponent<RocketBlock>();

            if (rocket == null)
            {
                Debug.LogError("Rocket prefab missing RocketBlock script");
                return;
            }

            RocketDirection direction = typeCode == "hro"
                ? RocketDirection.Horizontal
                : RocketDirection.Vertical;

            rocket.Initialize(direction);
        }


        string GetRandomColorCode()
        {
            string[] codes = { "r", "g", "b", "y" };
            return codes[Random.Range(0, codes.Length)];
        }

        BlockColor ParseColor(string code)
        {
            return code switch
            {
                "r" => BlockColor.Red,
                "g" => BlockColor.Green,
                "b" => BlockColor.Blue,
                "y" => BlockColor.Yellow,
                _ => BlockColor.Red
            };
        }
    }
}
