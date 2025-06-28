using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Blocks;
using Data;

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
        
        private CubeBlock[,] cubes;
        private ObstacleBlock[,] obstacles;
        
        private Vector3 gridOffset;


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
            
            cubes = new CubeBlock[width, height];
            obstacles = new ObstacleBlock[width, height];

            gridRoot = new GameObject("GridRoot").transform;
            grid = new CubeBlock[width, height];

            // Calculate grid offset for centering
            gridOffset = new Vector3(-(width - 1) * blockSize / 2f, -(height - 1) * blockSize / 2f, 0);

            for (int i = 0; i < data.grid.Count; i++)
            {
                int x = i % width;
                int y = i / width;

                Vector2Int pos = new Vector2Int(x, y);
                Vector3 worldPos = new Vector3(x * blockSize, y * blockSize, 0) + gridOffset;

                string type = data.grid[i] == "rand" ? GetRandomColorCode() : data.grid[i];

                // Check if it is obstacle, rocket or cube
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
            // TODO: fit the matrix shape and size later
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

            bgObj.transform.position = new Vector3(0, 0, 1); // put it behind of blocks
        }


        void CreateCube(BlockColor color, Vector2Int gridPos, Vector3 worldPos)
        {
            GameObject obj = Instantiate(cubePrefab, worldPos, Quaternion.identity, gridRoot);
            CubeBlock cube = obj.GetComponent<CubeBlock>();
            cube.Initialize(color, gridPos);
            grid[gridPos.x, gridPos.y] = cube;
            cubes[gridPos.x, gridPos.y] = cube;
        }
        
        private void CreateObstacle(string code, Vector2Int pos, Vector3 worldPos)
        {
            GameObject prefab = obstaclePrefab;
            GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, gridRoot);
            ObstacleBlock obstacle = obj.GetComponent<ObstacleBlock>();

            if (code == "bo") obstacle.Initialize(ObstacleType.Box, pos);
            else if (code == "s") obstacle.Initialize(ObstacleType.Stone, pos);
            else if (code == "v") obstacle.Initialize(ObstacleType.Vase, pos);

            obstacles[pos.x, pos.y] = obstacle;
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
        
        public void TryBlastGroupAt(Vector2Int origin)
        {
            CubeBlock startBlock = GetCubeAt(origin);
            if (startBlock == null) return;

            List<CubeBlock> group = FindConnectedCubes(startBlock);
            if (group.Count < 2) return;

            // Spend move here if needed

            StartCoroutine(BlastCubes(group));
        }
        
        private List<CubeBlock> FindConnectedCubes(CubeBlock start)
        {
            List<CubeBlock> result = new List<CubeBlock>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Queue<CubeBlock> queue = new Queue<CubeBlock>();
    
            queue.Enqueue(start);
            visited.Add(start.gridPosition);

            while (queue.Count > 0)
            {
                CubeBlock current = queue.Dequeue();
                result.Add(current);

                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighborPos = current.gridPosition + dir;

                    if (IsInBounds(neighborPos) && !visited.Contains(neighborPos))
                    {
                        CubeBlock neighbor = GetCubeAt(neighborPos);
                        if (neighbor != null && neighbor.color == start.color)
                        {
                            queue.Enqueue(neighbor);
                            visited.Add(neighborPos);
                        }
                    }
                }
            }

            return result;
        }

        // TODO: for rocket directions (rocket gameplay implement)
        private readonly List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        
        private void DropCubes()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    if (cubes[x, y] is not null)
                    {
                        int dropY = y;
                        while (dropY > 0)
                        {
                            if (obstacles[x, dropY - 1] is not null) break; // If there is an obstacle, stop
                            if (cubes[x, dropY - 1] is null)
                                dropY--;
                            else
                                break;
                        }

                        if (dropY != y)
                        {
                            CubeBlock block = cubes[x, y];
                            cubes[x, dropY] = block;
                            cubes[x, y] = null;

                            block.gridPosition = new Vector2Int(x, dropY);
                            block.transform.position = GetWorldPosition(x, dropY);
                        }
                    }
                }
            }
        }
        
        private void DropObstacles()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    ObstacleBlock obs = obstacles[x, y];
                    if (obs is not null && obs.CanFall() && obstacles[x, y - 1] is null && cubes[x, y - 1] is null)
                    {
                        int dropY = y;
                        while (dropY > 0 && obstacles[x, dropY - 1] is null && cubes[x, dropY - 1] is null)
                            dropY--;

                        obstacles[x, dropY] = obs;
                        obstacles[x, y] = null;

                        obs.gridPosition = new Vector2Int(x, dropY);
                        obs.transform.position = GetWorldPosition(x, dropY);
                    }
                }
            }
        }

        
        private void FillEmptySpaces()
        {
            for (int x = 0; x < width; x++)
            {
                int spawnY = height;

                for (int y = height - 1; y >= 0; y--)
                {
                    // Condition to check is there is an empty cell where is no any obstacle under it
                    if (cubes[x, y] is null && obstacles[x, y] is null)
                    {
                        // Create the cube to fall down
                        BlockColor randomColor = GetRandomColorEnum();
                        Vector2Int pos = new Vector2Int(x, y);
                        Vector3 spawnPos = GetWorldPosition(x, spawnY); // spawn above
                        Vector3 finalPos = GetWorldPosition(x, y);

                        GameObject obj = Instantiate(cubePrefab, spawnPos, Quaternion.identity, gridRoot);
                        CubeBlock newCube = obj.GetComponent<CubeBlock>();
                        newCube.Initialize(randomColor, pos);
                        cubes[x, y] = newCube;

                        newCube.transform.position = finalPos;

                        spawnY++; // jump to up of the cell to fall new blocks
                    }
                    else if (obstacles[x, y] is not null || cubes[x, y] is not null)
                    {
                        // If there is a cube or an obstacle, +1 spawn position on y-axis
                        spawnY = y + 1;
                    }
                }
            }
        }
        
        private IEnumerator BlastCubes(List<CubeBlock> group)
        {
            HashSet<Vector2Int> affectedObstaclePositions = new HashSet<Vector2Int>();
            
            foreach (var block in group)
            {
                string colorName = block.color.ToString().ToLower();
                Sprite particleSprite = Resources.Load<Sprite>($"Cubes/Particles/particle_{colorName}");

                GameObject particleObj = new GameObject("ParticleEffect");
                particleObj.transform.position = block.transform.position;

                SpriteRenderer sr = particleObj.AddComponent<SpriteRenderer>();
                sr.sprite = particleSprite;
                sr.sortingOrder = 5;

                Destroy(particleObj, 1f);

                // Clear from grid
                cubes[block.gridPosition.x, block.gridPosition.y] = null;
                Destroy(block.gameObject);

                // Check neighbors
                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighborPos = block.gridPosition + dir;
                    if (IsInBounds(neighborPos))
                    {
                        ObstacleBlock obs = obstacles[neighborPos.x, neighborPos.y];
                        if (obs is not null)
                        {
                            bool destroyed = obs.ApplyBlastDamage();
                            if (destroyed)
                                affectedObstaclePositions.Add(neighborPos);
                        }
                    }
                }
            }
            
            yield return new WaitForSeconds(0.4f);

            // Clear destroyed obstacles from grid
            foreach (var pos in affectedObstaclePositions)
            {
                obstacles[pos.x, pos.y] = null;
            }

            // Drop cubes and obstacles(vase) & refill the grid
            DropObstacles();
            DropCubes();
            FillEmptySpaces();
        }

        /*
         * UTILITY FUNCTIONS
         */
        
        private CubeBlock GetCubeAt(Vector2Int pos)
        {
            if (!IsInBounds(pos)) return null;
            return cubes[pos.x, pos.y];
        }

        private bool IsInBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
        }

        private Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x * blockSize, y * blockSize, 0) + gridOffset;
        }
        
        private BlockColor GetRandomColorEnum()
        {
            int value = Random.Range(0, 4);
            return (BlockColor)value; // Enum: 0=Red, 1=Green, 2=Blue, 3=Yellow
        }
    }
}
