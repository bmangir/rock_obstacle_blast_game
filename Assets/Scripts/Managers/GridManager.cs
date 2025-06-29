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
        private RocketBlock[,] rockets;
        
        private bool isProcessingMoves = false;
        
        private Vector3 gridOffset;


        void Start()
        {
            InitGrid();
            AdjustCameraToFitGrid();
            ShowAllRocketHints();
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
            rockets = new RocketBlock[width, height];

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
                    CreateRocket(type, pos);
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
        
        private void CreateRocket(string typeCode, Vector2Int gridPos)
        {
            Vector3 worldPos = GetWorldPosition(gridPos.x, gridPos.y);
            GameObject obj = Instantiate(rocketPrefab, worldPos, Quaternion.identity, gridRoot);
            RocketBlock rocket = obj.GetComponent<RocketBlock>();

            if (rocket is null)
            {
                Debug.LogError("Rocket prefab missing RocketBlock script");
                return;
            }

            // Check for starting level and in level to create rockets
            RocketDirection direction;
            if (typeCode != null)
            {

                direction = typeCode == "hro"
                    ? RocketDirection.Horizontal
                    : RocketDirection.Vertical;
            }

            else
            {
                direction = (Random.Range(0, 2) == 0 ? 
                    RocketDirection.Horizontal : 
                    RocketDirection.Vertical);
            }

            rocket.Initialize(direction, gridPos); 
            rockets[gridPos.x, gridPos.y] = rocket;
        }
        
        public void TryBlastGroupAt(Vector2Int origin)
        {
            // Wait for the animation finish to make move
            if (isProcessingMoves) return;
            
            CubeBlock startBlock = GetCubeAt(origin);
            if (startBlock == null) return;

            List<CubeBlock> group = FindConnectedCubes(startBlock);
            if (group.Count < 2) return;
            
            // TODO: Spend move here if needed

            isProcessingMoves = true;
            StartCoroutine(BlastCubes(group, origin));
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
                        if (neighbor is not null && neighbor.color == start.color)
                        {
                            queue.Enqueue(neighbor);
                            visited.Add(neighborPos);
                        }
                    }
                }
            }

            return result;
        }
        
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
                            if (obstacles[x, dropY - 1] is not null || 
                                cubes[x, dropY - 1] is not null ||
                                rockets[x, dropY - 1] is not null) 
                                break;
                            dropY--;
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
        
        private void DropRockets()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    if (rockets[x, y] is not null)
                    {
                        int dropY = y;
                        while (dropY > 0)
                        {
                            // Check if cell below is free
                            if (cubes[x, dropY - 1] is not null || 
                                obstacles[x, dropY - 1] is not null || 
                                rockets[x, dropY - 1] is not null)
                            {
                                break;
                            }
                            dropY--;
                        }

                        if (dropY != y)
                        {
                            RocketBlock rocket = rockets[x, y];
                            rockets[x, dropY] = rocket;
                            rockets[x, y] = null;

                            rocket.gridPosition = new Vector2Int(x, dropY);
                            rocket.transform.position = GetWorldPosition(x, dropY);
                        }
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
                    if (cubes[x, y] is null && 
                        obstacles[x, y] is null && 
                        rockets[x, y] is null)
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
        
        private IEnumerator BlastCubes(List<CubeBlock> group, Vector2Int origin)
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
            
            if (group.Count >= 4)
            {
                CreateRocket(null, origin);
            }

            // Drop cubes and obstacles(vase) & refill the grid
            DropObstacles();
            DropRockets();
            DropCubes();
            FillEmptySpaces();
    
            ShowAllRocketHints();
            isProcessingMoves = false;
        }
        
        public void BlastRocketAt(Vector2Int rocketPos)
        {
            if (isProcessingMoves) return;
            
            RocketBlock rocket = rockets[rocketPos.x, rocketPos.y];
            if (rocket == null) return;

            isProcessingMoves = true;
            StartCoroutine(ExplodeRocket(rocket));
        }
        
        private IEnumerator ExplodeRocket(RocketBlock rocket)
        {
            // Remove rocket from grid
            rockets[rocket.gridPosition.x, rocket.gridPosition.y] = null;
            
            // Check for adjacent rockets for combo
            List<RocketBlock> adjacentRockets = FindAdjacentRockets(rocket.gridPosition);
            bool isCombo = adjacentRockets.Count > 0;

            if (isCombo)
            {
                // Combo explosion: 3x3 area
                // TODO: ask if the chain reaction of rocket apply the vertical and horizontal blasts by the dir of rockets
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector2Int pos = rocket.gridPosition + new Vector2Int(x, y);
                        if (IsInBounds(pos))
                        {
                            DamageCell(pos, true);
                        }
                    }
                }
                
                // Blast adjacent rockets recursively
                foreach (RocketBlock adjRocket in adjacentRockets)
                {
                    if (rockets[adjRocket.gridPosition.x, adjRocket.gridPosition.y] is not null)
                    {
                        rockets[adjRocket.gridPosition.x, adjRocket.gridPosition.y] = null;
                        yield return StartCoroutine(ExplodeRocket(adjRocket));
                    }
                }
            }
            else
            {
                if (rocket.direction == RocketDirection.Horizontal)
                {
                    // Left direction
                    for (int x = rocket.gridPosition.x; x >= 0; x--)
                    {
                        if (!DamageCell(new Vector2Int(x, rocket.gridPosition.y), false))
                            break;
                    }
                    
                    // Right direction
                    for (int x = rocket.gridPosition.x + 1; x < width; x++)
                    {
                        if (!DamageCell(new Vector2Int(x, rocket.gridPosition.y), false))
                            break;
                    }
                }
                else
                {
                    // Down direction
                    for (int y = rocket.gridPosition.y; y >= 0; y--)
                    {
                        if (!DamageCell(new Vector2Int(rocket.gridPosition.x, y), false))
                            break;
                    }
                    
                    // Up direction
                    for (int y = rocket.gridPosition.y + 1; y < height; y++)
                    {
                        if (!DamageCell(new Vector2Int(rocket.gridPosition.x, y), false))
                            break;
                    }
                }
            }

            Destroy(rocket.gameObject);
            yield return new WaitForSeconds(0.4f);

            DropObstacles();
            DropRockets();
            DropCubes();
            FillEmptySpaces();
            
            ShowAllRocketHints();
            isProcessingMoves = false;
        }
        
        private bool DamageCell(Vector2Int pos, bool isCombo)
        {
            // Damage cubes
            CubeBlock cube = cubes[pos.x, pos.y];
            if (cube is not null)
            {
                cubes[pos.x, pos.y] = null;
                Destroy(cube.gameObject);
            }

            // Damage obstacles
            ObstacleBlock obs = obstacles[pos.x, pos.y];
            if (obs is not null)
            {
                bool destroyed = obs.ApplyRocketDamage();
                if (destroyed)
                {
                    obstacles[pos.x, pos.y] = null;
                }
                
                // Stone stops rockets
                if (obs.obstacleType == ObstacleType.Stone && !isCombo)
                    return false;
            }

            // Check for rockets (non-combo only)
            if (!isCombo)
            {
                RocketBlock rocket = rockets[pos.x, pos.y];
                if (rocket is not null)
                {
                    // Chain reaction
                    rockets[pos.x, pos.y] = null;
                    StartCoroutine(ExplodeRocket(rocket));
                    return false;
                }
            }

            return true;
        }
        
        private List<RocketBlock> FindAdjacentRockets(Vector2Int position)
        {
            List<RocketBlock> result = new List<RocketBlock>();
            
            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighborPos = position + dir;
                if (IsInBounds(neighborPos))
                {
                    RocketBlock rocket = rockets[neighborPos.x, neighborPos.y];
                    if (rocket is not null)
                    {
                        result.Add(rocket);
                    }
                }
            }
            
            return result;
        }
        
        private void ShowAllRocketHints()
        {
            // Clear all hints
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (cubes[x, y] is not null)
                    {
                        cubes[x, y].HideRocketHint();
                    }
                }
            }

            // Find and show eligible groups
            // TODO: process with fill the empty cells from above and initialization first grid 
            HashSet<CubeBlock> processed = new HashSet<CubeBlock>();
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    CubeBlock cube = cubes[x, y];
                    if (cube is not null && !processed.Contains(cube))
                    {
                        List<CubeBlock> group = FindConnectedCubes(cube);
                        if (group.Count >= 4)
                        {
                            foreach (CubeBlock block in group)
                            {
                                block.ShowRocketHint();
                                processed.Add(block);
                            }
                        }
                    }
                }
            }
        }

        /*
         * UTILITY FUNCTIONS
         */
        
        private readonly List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        
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
