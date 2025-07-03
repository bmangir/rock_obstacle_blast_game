using System.Collections;
using System.Linq;
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
        
        private float totalGridWidth;
        private float totalGridHeight;
        
        [SerializeField] private float blockSize = 0.6f;
        [SerializeField] private float blockSpacing = 0.05f;
        [SerializeField] private float topPanelMargin = 2.5f; // World units
        [SerializeField] private Vector2 backgroundPadding = new Vector2(1f, 1f);
        
        private CubeBlock[,] cubes;
        private ObstacleBlock[,] obstacles;
        private RocketBlock[,] rockets;
        
                private bool isProcessingMoves = false;
        
        // Track objects currently animating to prevent conflicts
        private HashSet<Transform> animatingTransforms = new HashSet<Transform>();

        private Vector3 gridOffset;

        private bool isInitialized = false;

        private GoalPanelManager goalPanelManager;
        private Dictionary<ObstacleType, int> obstacleGoals = new Dictionary<ObstacleType, int>();


        void Start()
        {
            InitGrid();
            StartCoroutine(ShowInitialHints());
        }
        
        private IEnumerator ShowInitialHints()
        {
            yield return null; // Wait for initialization to complete
            ShowAllRocketHints();
            isInitialized = true;
        }
        
        void AdjustCameraToFitGrid()
        {
            Camera cam = Camera.main;
            float aspectRatio = (float)Screen.width / Screen.height;

            // Calculate required orthographic size
            float requiredWidth = totalGridWidth / aspectRatio;
            float requiredHeight = (totalGridHeight + topPanelMargin) * 0.5f;

            cam.orthographicSize = Mathf.Max(requiredHeight, requiredWidth / (2 * aspectRatio));

            // Position camera to center grid below top panel
            float cameraY = cam.orthographicSize - (topPanelMargin * 0.5f) - (totalGridHeight * 0.5f);
            cam.transform.position = new Vector3(0, cameraY, -10);
        }
        

        void InitGrid()
        {
            var oldGrid = GameObject.Find("GridRoot");
            if (oldGrid != null) Destroy(oldGrid);
            
            LevelData data = levelLoader.levelData;
            width = data.grid_width;
            height = data.grid_height;
            
            totalGridWidth = width * (blockSize + blockSpacing) - blockSpacing;
            totalGridHeight = height * (blockSize + blockSpacing) - blockSpacing;
            Debug.Log($"Width: {width}, Height: {height}");
            Debug.Log($"TotalGridWidth: {totalGridWidth}, TotalGridHeight: {totalGridHeight}");
            
            float centerOffsetX = -totalGridWidth / 2f;
            float centerOffsetY = -totalGridHeight / 2f;
            gridOffset = new Vector3(centerOffsetX, centerOffsetY, 0);
            
            AdjustCameraToFitGrid();
            
            cubes = new CubeBlock[width, height];
            obstacles = new ObstacleBlock[width, height];
            rockets = new RocketBlock[width, height];
            
            obstacleGoals[ObstacleType.Box] = 0;
            obstacleGoals[ObstacleType.Stone] = 0;
            obstacleGoals[ObstacleType.Vase] = 0;

            gridRoot = new GameObject("GridRoot").transform;
            grid = new CubeBlock[width, height];
            
            goalPanelManager = FindObjectOfType<GoalPanelManager>();

            for (int i = 0; i < data.grid.Count; i++)
            {
                int x = i % width;
                int y = i / width;
                Vector2Int pos = new Vector2Int(x, y);
                Vector3 worldPos = new Vector3(
                    x * (blockSize + blockSpacing),
                    y * (blockSize + blockSpacing),
                    0
                ) + gridOffset;
                
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
            InitializeGoalPanel(data);
            
            CreateGridBackground();
        }
        
        private void InitializeGoalPanel(LevelData data)
        {
            var nonZeroGoals = obstacleGoals
                .Where(kvp => kvp.Value > 0)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    
            goalPanelManager.Initialize(nonZeroGoals, data.move_count);
        }
        
        void CreateGridBackground()
        {
            CreateGridSpecificBackground();
        }
        
        void CreateGridSpecificBackground()
        {
            Sprite gridBgSprite = Resources.Load<Sprite>("UI/Gameplay/grid_background");
            if (gridBgSprite == null)
            {
                Debug.LogError("Grid background sprite not found at Resources/UI/Gameplay/grid_background.png");
                return;
            }

            GameObject gridBgObj = new GameObject("GridBackground");
            SpriteRenderer sr = gridBgObj.AddComponent<SpriteRenderer>();
            sr.sprite = gridBgSprite;
            sr.sortingOrder = -1; // Just behind grid blocks 
            
            // Calculate the exact grid area size // TODO: fix this 'cause it's not matching
            float gridWorldWidth = totalGridWidth + (backgroundPadding.x * 2);
            float gridWorldHeight = totalGridHeight + (backgroundPadding.y * 2);
            
            // Use uniform scaling to maintain circular shape
            float maxDimension = Mathf.Max(gridWorldWidth, gridWorldHeight);
            float uniformScale = (maxDimension * 1.2f) / Mathf.Min(sr.sprite.bounds.size.x, sr.sprite.bounds.size.y);
            
            gridBgObj.transform.localScale = new Vector3(uniformScale, uniformScale, 1f);
            
            // Position it at the center of the grid // Match with the cameranot world space
            gridBgObj.transform.position = new Vector3(gridOffset.x, gridOffset.y, 5f);
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

            if (code == "bo")
            {
                obstacle.Initialize(ObstacleType.Box, pos);
                obstacleGoals[ObstacleType.Box]++;
            }
            else if (code == "s")
            {
                obstacle.Initialize(ObstacleType.Stone, pos);
                obstacleGoals[ObstacleType.Stone]++;
            }
            else if (code == "v")
            {
                obstacle.Initialize(ObstacleType.Vase, pos);
                obstacleGoals[ObstacleType.Vase]++;
            }

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
            if (startBlock is null) return;

            List<CubeBlock> group = FindConnectedCubes(startBlock);
            if (group.Count < 2) return;

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
        
        private bool DropCubes()
        {
            bool somethingDropped = false;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    if (cubes[x, y] is not null)
                    {
                        CubeBlock cube = cubes[x, y];
                        int dropY = y;
                        
                        // Find how far this cube can drop
                        while (dropY > 0)
                        {
                            // Check if the position below is blocked
                            if (cubes[x, dropY - 1] is not null || 
                                rockets[x, dropY - 1] is not null || 
                                obstacles[x, dropY - 1] is not null)
                            {
                                break; // Can't drop further
                            }
                            dropY--;
                        }
                        
                        // Move the cube if it can drop
                        if (dropY != y)
                        {
                            cubes[x, y] = null;
                            cubes[x, dropY] = cube;
                            cube.gridPosition = new Vector2Int(x, dropY);
                            StartCoroutine(AnimateBlockFall(cube.transform, GetWorldPosition(x, dropY)));
                            somethingDropped = true;
                        }
                    }
                }
            }
            
            return somethingDropped;
        }
        
        private bool DropObstacles()
        {
            bool somethingDropped = false;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    ObstacleBlock obs = obstacles[x, y];
                    // Only Vase can fall
                    if (obs is not null && obs.CanFall())
                    {
                        int dropY = y;
                        
                        // Find how far this obstacle can drop
                        while (dropY > 0)
                        {
                            // Check if the position below is blocked
                            if (obstacles[x, dropY - 1] is not null || 
                                cubes[x, dropY - 1] is not null || 
                                rockets[x, dropY - 1] is not null)
                            {
                                break; // Can't drop further
                            }
                            dropY--;
                        }
                        
                        // Move the obstacle if it can drop
                        if (dropY != y)
                        {
                            obstacles[x, y] = null;
                            obstacles[x, dropY] = obs;
                            obs.gridPosition = new Vector2Int(x, dropY);
                            
                            // Use enhanced animation for obstacles with different effects for vase
                            StartCoroutine(AnimateObstacleFall(obs.transform, GetWorldPosition(x, dropY), obs.obstacleType));
                            somethingDropped = true;
                        }
                    }
                }
            }
            
            return somethingDropped;
        }
        
        private IEnumerator AnimateObstacleFall(Transform obstacleTransform, Vector3 targetPosition, ObstacleType obstacleType)
        {
            if (obstacleTransform is null) yield break;
            
            // Check if already animating to prevent conflicts
            if (animatingTransforms.Contains(obstacleTransform)) yield break;
            
            // Add to tracking set
            animatingTransforms.Add(obstacleTransform);
            
            Vector3 startPosition = obstacleTransform.position;
            float fallDistance = startPosition.y - targetPosition.y;
            
            // Different animation styles based on obstacle type
            float baseDuration = 0.45f; // Slightly slower than cubes
            float duration = Mathf.Max(0.25f, baseDuration + (fallDistance * 0.06f));
            
            float elapsed = 0f;
            Vector3 originalScale = obstacleTransform.localScale;
            
            // Vases get different animation than other obstacles
            bool isVase = obstacleType == ObstacleType.Vase;
            float wobbleIntensity = isVase ? 0.03f : 0.01f; // Vases wobble more
            float rotationSpeed = isVase ? Random.Range(-90f, 90f) : Random.Range(-45f, 45f);
            
            while (elapsed < duration)
            {
                if (obstacleTransform == null) 
                {
                    animatingTransforms.Remove(obstacleTransform);
                    yield break;
                }
                
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Slightly different easing for obstacles (more realistic weight)
                float easeT;
                if (t < 0.6f)
                {
                    easeT = t * t * 1.6f; // Heavier acceleration
                }
                else
                {
                    float bounceT = (t - 0.6f) / 0.4f;
                    easeT = 0.6f * 0.6f * 1.6f + bounceT * bounceT * 0.4f;
                }
                
                Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, easeT);
                
                // Add wobble effect (vases wobble more as they're fragile)
                float wobbleOffset = Mathf.Sin(elapsed * 12f) * wobbleIntensity * (1f - t);
                currentPos.x += wobbleOffset;
                
                obstacleTransform.position = currentPos;
                
                float scaleEffect = 1f + Mathf.Sin(elapsed * 8f) * 0.03f * (1f - t);
                obstacleTransform.localScale = originalScale * scaleEffect;
                
                // Rotation effect (vases rotate more)
                float currentRotation = rotationSpeed * elapsed * (1f - t);
                obstacleTransform.rotation = Quaternion.Euler(0, 0, currentRotation);
                
                yield return null;
            }

            // Landing effect
            if (obstacleTransform != null)
            {
                yield return StartCoroutine(AnimateObstacleLanding(obstacleTransform, targetPosition, originalScale, isVase));
            }
            
            if (animatingTransforms.Contains(obstacleTransform))
            {
                animatingTransforms.Remove(obstacleTransform);
            }
        }
        
        private IEnumerator AnimateObstacleLanding(Transform obstacleTransform, Vector3 targetPosition, Vector3 originalScale, bool isVase)
        {
            if (obstacleTransform == null) yield break;
            
            float bounceTime = isVase ? 0.2f : 0.12f; // Vases bounce longer
            float elapsed = 0f;
            
            while (elapsed < bounceTime)
            {
                if (obstacleTransform == null) yield break;
                
                elapsed += Time.deltaTime;
                float t = elapsed / bounceTime;
                
                // Different bounce patterns for different obstacles
                float bounceIntensity = isVase ? 0.12f : 0.08f;
                float bounceScale = 1f + Mathf.Sin(t * Mathf.PI) * bounceIntensity;
                obstacleTransform.localScale = originalScale * bounceScale;
                
                // Landing impact
                float impactOffset = Mathf.Sin(t * Mathf.PI) * (isVase ? 0.025f : 0.015f);
                obstacleTransform.position = targetPosition + Vector3.down * impactOffset;
                
                yield return null;
            }
            
            if (obstacleTransform != null)
            {
                obstacleTransform.position = targetPosition;
                obstacleTransform.localScale = originalScale;
                obstacleTransform.rotation = Quaternion.identity;
                
                StartCoroutine(CreateObstacleLandingEffect(targetPosition, isVase));
            }
        }
        
        private IEnumerator CreateObstacleLandingEffect(Vector3 position, bool isVase)
        {
            GameObject landingEffect = new GameObject(isVase ? "VaseLanding" : "ObstacleLanding");
            landingEffect.transform.position = position;
            
            ParticleSystem particles = landingEffect.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.startLifetime = isVase ? 0.4f : 0.25f;
            main.startSpeed = isVase ? 1.5f : 0.8f;
            main.startSize = isVase ? 0.06f : 0.04f;
            main.maxParticles = isVase ? 12 : 6;
            
            if (isVase)
            {
                main.startColor = new Color(0.9f, 0.8f, 0.6f, 0.6f); // Dusty/ceramic color
            }
            else
            {
                main.startColor = new Color(0.7f, 0.7f, 0.7f, 0.5f); // Stone/box dust
            }
            
            var emission = particles.emission;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0.0f, isVase ? 12 : 6)
            });
            
            var shape = particles.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = isVase ? 0.12f : 0.08f;
            
            var velocityOverLifetime = particles.velocityOverLifetime;
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
            velocityOverLifetime.radial = new ParticleSystem.MinMaxCurve(isVase ? 2.5f : 1.5f);
            
            Destroy(landingEffect, 1f);
            yield return null;
        }
        
        private IEnumerator DropAllUntilStable()
        {
            bool stillDropping = true;
            int maxIterations = 10; // Prevent infinite loops
            int iteration = 0;
            
            while (stillDropping && iteration < maxIterations)
            {
                stillDropping = false;
                
                // Drop vases first (highest priority)
                if (DropObstacles())
                {
                    // To avoid conflict between cubes rockets and vase
                    stillDropping = true;
                    yield return new WaitForSeconds(0.15f);
                }
                
                // Then drop rockets
                if (DropRockets())
                {
                    stillDropping = true;
                    yield return new WaitForSeconds(0.15f);
                }
                
                // Finally drop cubes
                if (DropCubes())
                {
                    stillDropping = true;
                    yield return new WaitForSeconds(0.15f);
                }
                
                iteration++;
            }
            
            yield return new WaitForSeconds(0.2f);
        }
        
        private bool DropRockets()
        {
            bool somethingDropped = false;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    if (rockets[x, y] is not null)
                    {
                        RocketBlock rocket = rockets[x, y];
                        int dropY = y;
                        
                        // Find how far this rocket can drop
                        while (dropY > 0)
                        {
                            // Check if the position below is blocked
                            if (cubes[x, dropY - 1] is not null || 
                                rockets[x, dropY - 1] is not null || 
                                obstacles[x, dropY - 1] is not null)
                            {
                                break; // Can't drop further
                            }
                            dropY--;
                        }
                        
                        // Move the rocket if it can drop
                        if (dropY != y)
                        {
                            rockets[x, y] = null;
                            rockets[x, dropY] = rocket;
                            rocket.gridPosition = new Vector2Int(x, dropY);
                            StartCoroutine(AnimateBlockFall(rocket.transform, GetWorldPosition(x, dropY)));
                            somethingDropped = true;
                        }
                    }
                }
            }
            
            return somethingDropped;
        }

        
        private void FillEmptySpaces()
        {
            List<CubeBlock> newBlocks = new List<CubeBlock>();
            
            for (int x = 0; x < width; x++)
            {
                int spawnY = height;

                for (int y = height - 1; y >= 0; y--)
                {
                    // Condition to check is there is an empty cell where is no any obstacle under it
                    // (For stone and box)
                    if (cubes[x, y] is null && 
                        obstacles[x, y] is null && 
                        rockets[x, y] is null)
                    {
                        // Create the cube to fall down with enhanced spawning
                        BlockColor randomColor = GetRandomColorEnum();
                        Vector2Int pos = new Vector2Int(x, y);
                        Vector3 spawnPos = GetWorldPosition(x, height + 2); // spawn at medium height
                        Vector3 finalPos = GetWorldPosition(x, y);

                        GameObject obj = Instantiate(cubePrefab, spawnPos, Quaternion.identity, gridRoot);
                        CubeBlock newCube = obj.GetComponent<CubeBlock>();
                        newCube.Initialize(randomColor, pos);
                        cubes[x, y] = newCube;

                        // Add initial spawn effects
                        StartCoroutine(AnimateNewBlockSpawn(newCube, spawnPos, finalPos));
                        newBlocks.Add(newCube);

                        spawnY++; // jump to up of the cell to fall new blocks
                    }
                    else if (obstacles[x, y] is not null || cubes[x, y] is not null)
                    {
                        // If there is a cube or an obstacle, +1 spawn position on y-axis
                        spawnY = y + 1;
                    }
                }
            }
            
            // Start cascading effect for all new blocks
            if (newBlocks.Count > 0)
            {
                StartCoroutine(CascadingNewBlockEffect(newBlocks));
            }
            
            StartCoroutine(DelayedHintUpdate());
        }
        
        private IEnumerator AnimateNewBlockSpawn(CubeBlock newBlock, Vector3 spawnPos, Vector3 finalPos)
        {
            if (newBlock == null) yield break;
            
            Transform blockTransform = newBlock.transform;
            Vector3 originalScale = blockTransform.localScale;
            
            // Position at moderate height above grid
            Camera cam = Camera.main;
            if (cam != null)
            {
                float cameraTop = cam.transform.position.y + cam.orthographicSize;
                spawnPos.y = Mathf.Min(spawnPos.y, cameraTop + 1.2f); // Medium height above camera view
            }
            
            // Start with normal scale but positioned just off-screen
            blockTransform.localScale = originalScale;
            blockTransform.position = spawnPos;
            
            // Small delay
            yield return new WaitForSeconds(Random.Range(0f, 0.06f));
            
            if (blockTransform != null)
            {
                StartCoroutine(AnimateBlockFall(blockTransform, finalPos));
            }
        }
        
        private IEnumerator CascadingNewBlockEffect(List<CubeBlock> newBlocks)
        {
            // Group blocks by column for wave effect
            var columnGroups = new Dictionary<int, List<CubeBlock>>();
            
            foreach (var block in newBlocks)
            {
                int x = block.gridPosition.x;
                if (!columnGroups.ContainsKey(x))
                {
                    columnGroups[x] = new List<CubeBlock>();
                }
                columnGroups[x].Add(block);
            }
            
            // Create wave effect from center outward
            int centerX = width / 2;
            var sortedColumns = columnGroups.Keys.OrderBy(x => Mathf.Abs(x - centerX)).ToArray();
            
            foreach (int x in sortedColumns)
            {
                // Delay between columns for wave effect
                yield return new WaitForSeconds(0.04f);
                
                // Add subtle sparkle effect at top of each column
                Vector3 columnTop = GetWorldPosition(x, height);
                
                // Keep sparkle within camera view
                Camera cam = Camera.main;
                if (cam != null)
                {
                    float maxY = cam.transform.position.y + cam.orthographicSize - 0.5f;
                    columnTop.y = Mathf.Min(columnTop.y, maxY);
                }
                
                StartCoroutine(CreateColumnSparkle(columnTop));
            }
        }
        
        private IEnumerator CreateColumnSparkle(Vector3 position)
        {
            GameObject sparkle = new GameObject("ColumnSparkle");
            sparkle.transform.position = position;
            
            ParticleSystem particles = sparkle.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.startLifetime = 0.5f;
            main.startSpeed = 2f;
            main.startSize = 0.08f;
            main.startColor = new Color(0.8f, 0.9f, 1f, 0.8f);
            main.maxParticles = 12;
            
            var emission = particles.emission;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0.0f, 12)
            });
            
            var shape = particles.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 25f;
            shape.radius = 0.05f;
            
            Destroy(sparkle, 1f);
            yield return null;
        }
        
        private IEnumerator AnimateBlockFall(Transform blockTransform, Vector3 targetPosition)
        {
            if (blockTransform is null) yield break;
            
            // Check if already animating to prevent conflicts
            if (animatingTransforms.Contains(blockTransform)) yield break;
            
            // Add to tracking set
            animatingTransforms.Add(blockTransform);
            
            Vector3 startPosition = blockTransform.position;
            float fallDistance = startPosition.y - targetPosition.y;
            
            // Balanced animation speed
            float baseDuration = 0.35f;
            float duration = Mathf.Max(0.2f, baseDuration + (fallDistance * 0.03f));
            
            float elapsed = 0f;
            
            Vector3 originalScale = blockTransform.localScale;
            float rotationSpeed = Random.Range(-120f, 120f);
            
            // Make block invisible if it's outside camera view initially
            SpriteRenderer spriteRenderer = blockTransform.GetComponent<SpriteRenderer>();
            bool wasVisible = true;
            if (spriteRenderer != null)
            {
                Camera cam = Camera.main;
                if (cam != null)
                {
                    float cameraTop = cam.transform.position.y + cam.orthographicSize;
                    if (startPosition.y > cameraTop)
                    {
                        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
                        wasVisible = false;
                    }
                }
            }
            
            // Small delay
            float initialDelay = Random.Range(0f, 0.04f);
            yield return new WaitForSeconds(initialDelay);

            while (elapsed < duration)
            {
                // Check if transform still exists before accessing it
                if (blockTransform == null) 
                {
                    animatingTransforms.Remove(blockTransform);
                    yield break;
                }
                
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Enhanced easing with anticipation and overshoot
                float easeT;
                if (t < 0.7f)
                {
                    // Accelerating fall with gravity effect
                    easeT = t * t * 1.4f; 
                }
                else
                {
                    // Deceleration with slight bounce preparation
                    float bounceT = (t - 0.7f) / 0.3f;
                    easeT = 0.7f * 0.7f * 1.4f + bounceT * bounceT * 0.3f;
                }
                
                // Position interpolation
                Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, easeT);
                
                // Add nice horizontal wobble during fall
                float wobbleIntensity = 0.025f * (1f - t); // Wobble decreases as it approaches target
                float wobbleOffset = Mathf.Sin(elapsed * 12f) * wobbleIntensity;
                currentPos.x += wobbleOffset;
                
                blockTransform.position = currentPos;
                
                // Make block visible when it enters camera view
                if (!wasVisible && spriteRenderer != null)
                {
                    Camera cam = Camera.main;
                    if (cam != null)
                    {
                        float cameraTop = cam.transform.position.y + cam.orthographicSize;
                        if (currentPos.y <= cameraTop)
                        {
                            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                            wasVisible = true;
                        }
                    }
                }
                
                float scaleEffect = 1f + Mathf.Sin(elapsed * 9f) * 0.06f * (1f - t);
                blockTransform.localScale = originalScale * scaleEffect;
                
                float currentRotation = rotationSpeed * elapsed * (1f - t);
                blockTransform.rotation = Quaternion.Euler(0, 0, currentRotation);
                
                yield return null;
            }
            
            if (blockTransform != null)
            {
                yield return StartCoroutine(AnimateBounceLanding(blockTransform, targetPosition, originalScale));
            }
            
            if (animatingTransforms.Contains(blockTransform))
            {
                animatingTransforms.Remove(blockTransform);
            }
        }
        
        private IEnumerator AnimateBounceLanding(Transform blockTransform, Vector3 targetPosition, Vector3 originalScale)
        {
            if (blockTransform == null) yield break;
            
            // Landing bounce effect
            float bounceTime = 0.12f;
            float elapsed = 0f;
            
            while (elapsed < bounceTime)
            {
                if (blockTransform == null) yield break;
                
                elapsed += Time.deltaTime;
                float t = elapsed / bounceTime;
                
                // Bounce scale effect
                float bounceScale = 1f + Mathf.Sin(t * Mathf.PI) * 0.15f;
                blockTransform.localScale = originalScale * bounceScale;
                
                float impactOffset = Mathf.Sin(t * Mathf.PI) * 0.02f;
                blockTransform.position = targetPosition + Vector3.down * impactOffset;
                
                yield return null;
            }
            
            // Ensure final state
            if (blockTransform != null)
            {
                blockTransform.position = targetPosition;
                blockTransform.localScale = originalScale;
                blockTransform.rotation = Quaternion.identity;
                
                // Ensure block is fully visible at the end
                SpriteRenderer finalSR = blockTransform.GetComponent<SpriteRenderer>();
                if (finalSR != null)
                {
                    finalSR.color = new Color(finalSR.color.r, finalSR.color.g, finalSR.color.b, 1f);
                }
                
                StartCoroutine(CreateLandingEffect(targetPosition));
            }
        }
        
        private IEnumerator CreateLandingEffect(Vector3 position)
        {
            // Create small dust particles on landing
            if (ParticleEffectManager.Instance != null)
            {
                // Small sparkle effect on landing
                GameObject sparkleEffect = new GameObject("LandingSparkle");
                sparkleEffect.transform.position = position;
                
                ParticleSystem particles = sparkleEffect.AddComponent<ParticleSystem>();
                var main = particles.main;
                main.startLifetime = 0.3f;
                main.startSpeed = 1f;
                main.startSize = 0.05f;
                main.startColor = new Color(1f, 1f, 1f, 0.7f);
                main.maxParticles = 8;
                
                var emission = particles.emission;
                emission.SetBursts(new ParticleSystem.Burst[]
                {
                    new ParticleSystem.Burst(0.0f, 8)
                });
                emission.enabled = true;
                
                var shape = particles.shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Circle;
                shape.radius = 0.1f;
                
                var velocityOverLifetime = particles.velocityOverLifetime;
                velocityOverLifetime.enabled = true;
                velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
                velocityOverLifetime.radial = new ParticleSystem.MinMaxCurve(2f);
                
                // Auto-destroy after effect
                Destroy(sparkleEffect, 1f);
            }
            
            yield return null;
        }
        
        public void StartCascadingBlockFalls()
        {
            StartCoroutine(CascadingFallSequence());
        }
        
        private IEnumerator CascadingFallSequence()
        {
            for (int x = 0; x < width; x++)
            {
                bool columnHasMovement = false;
                
                for (int y = height - 1; y >= 0; y--)
                {
                    if (cubes[x, y] != null)
                    {
                        // Add subtle delay between columns for wave effect
                        if (columnHasMovement)
                        {
                            yield return new WaitForSeconds(0.02f);
                        }
                        columnHasMovement = true;
                    }
                }
            }
        }
        
        private IEnumerator DelayedHintUpdate()
        {
            yield return new WaitForSeconds(0.5f);
            ShowAllRocketHints();
        }
        
        private IEnumerator BlastCubes(List<CubeBlock> group, Vector2Int origin)
        {
            HashSet<Vector2Int> affectedObstaclePositions = new HashSet<Vector2Int>();
            HashSet<Vector2Int> obstacleNeighbors = new HashSet<Vector2Int>(); // Track unique obstacles to damage
            
            foreach (var block in group)
            {
                StartCoroutine(AnticipationEffect(block.transform));
            }
            
            yield return new WaitForSeconds(0.1f);
            
            CameraShakeManager.Instance.ShakeForCubeBlast(group.Count);
            
            foreach (var block in group)
            {
                string colorName = block.color.ToString().ToLower();
                Vector3 blockPosition = block.transform.position;
                
                Color cubeColor = GetCubeColor(block.color);
                
                ParticleEffectManager.Instance.CreateCubeBlastEffect(blockPosition, cubeColor, colorName);

                cubes[block.gridPosition.x, block.gridPosition.y] = null;
                Destroy(block.gameObject);

                // Collect unique obstacle neighbors
                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighborPos = block.gridPosition + dir;
                    if (IsInBounds(neighborPos))
                    {
                        ObstacleBlock obs = obstacles[neighborPos.x, neighborPos.y];
                        if (obs is not null)
                        {
                            obstacleNeighbors.Add(neighborPos);
                        }
                    }
                }
            }
            
            // Apply damage only once per unique obstacle
            foreach (var obstaclePos in obstacleNeighbors)
            {
                ObstacleBlock obs = obstacles[obstaclePos.x, obstaclePos.y];
                if (obs is not null)
                {
                    bool destroyed = obs.ApplyBlastDamage();
                    if (destroyed)
                    {
                        goalPanelManager.DecrementObstacle(obs.obstacleType);
                        affectedObstaclePositions.Add(obstaclePos);
                    }
                }
            }
            
            yield return new WaitForSeconds(0.4f);

            foreach (var pos in affectedObstaclePositions)
            {
                obstacles[pos.x, pos.y] = null;
            }
            
            if (group.Count >= 4)
            {
                CreateRocket(null, origin);
            }

            // Drop everything until stable
            yield return StartCoroutine(DropAllUntilStable());
            
            // Fill empty spaces immediately
            FillEmptySpaces();
            
            // Wait for all falling animations to complete
            yield return new WaitForSeconds(0.5f);
            
            goalPanelManager.DecrementMove();
            
            goalPanelManager.CheckWinLoseAfterMove();
            
            isProcessingMoves = false;
        }
        
        public void BlastRocketAt(Vector2Int rocketPos)
        {
            if (isProcessingMoves) return;
            
            RocketBlock rocket = rockets[rocketPos.x, rocketPos.y];
            if (rocket == null) return;
            
            // Check for adjacent rockets for combo
            List<RocketBlock> adjacentRockets = FindAdjacentRockets(rocketPos);
            
            isProcessingMoves = true;
            
            if (adjacentRockets.Count >= 1) // At least 1 adjacent rocket for combo
            {
                Debug.Log($"ROCKET COMBO: {adjacentRockets.Count + 1} adjacent rockets - 3x3 + directional explosions");
                StartCoroutine(ExplodeRocketCombo(rocket, adjacentRockets));
            }
            else
            {
                Debug.Log("SINGLE ROCKET: Normal directional explosion only");
                StartCoroutine(ExplodeRocket(rocket));
            }
        }
        
        private IEnumerator ExplodeRocketCombo(RocketBlock mainRocket, List<RocketBlock> adjacentRockets)
        {
            // All rockets in combo
            List<RocketBlock> allRockets = new List<RocketBlock> { mainRocket };
            allRockets.AddRange(adjacentRockets);

            CameraShakeManager.Instance.ShakeForComboBlast(allRockets.Count);

            foreach (var rocket in allRockets)
            {
                rockets[rocket.gridPosition.x, rocket.gridPosition.y] = null;
            }

            List<Vector3> rocketPositions = new List<Vector3>();
            List<RocketDirection> rocketDirections = new List<RocketDirection>();
            
            // Collect rocket positions and start individual effects
            foreach (var rocket in allRockets)
            {
                Vector3 centerPos = GetWorldPosition(rocket.gridPosition.x, rocket.gridPosition.y);
                rocketPositions.Add(centerPos);
                rocketDirections.Add(rocket.direction);
                
                StartCoroutine(AnimateRocketParts(centerPos, rocket.direction));
                
                Destroy(rocket.gameObject);
            }
            
            StartCoroutine(CreateSpectacularComboSequence(rocketPositions, rocketDirections));

            // Immediate 3x3 explosion around each rocket
            foreach (var rocket in allRockets)
            {
                Vector2Int center = rocket.gridPosition;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        Vector2Int targetPos = new Vector2Int(center.x + dx, center.y + dy);
                        if (IsInBounds(targetPos))
                        {
                            DamageCell(targetPos, true, true); // true = combo damage, true = combo explosion
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.05f); 
            
            foreach (var rocket in allRockets)
            {
                StartCoroutine(RocketBlastEffect(rocket.direction, rocket.gridPosition));
            }
            
            yield return new WaitForSeconds(0.15f);

            // Immediately drop everything after blast effects
            yield return StartCoroutine(DropAllUntilStable());
            
            FillEmptySpaces();
            
            yield return new WaitForSeconds(0.3f);

            goalPanelManager.DecrementMove();
            
            goalPanelManager.CheckWinLoseAfterMove();
            
            isProcessingMoves = false;
        }

        private IEnumerator ExplodeRocket(RocketBlock rocket)
        {
            rockets[rocket.gridPosition.x, rocket.gridPosition.y] = null;

            Destroy(rocket.gameObject);

            Vector3 centerPos = GetWorldPosition(rocket.gridPosition.x, rocket.gridPosition.y);
            
            CreateRocketSmoke(centerPos);
            
            StartCoroutine(AnimateRocketParts(centerPos, rocket.direction));
            yield return StartCoroutine(RocketBlastEffect(rocket.direction, rocket.gridPosition));

            // Immediately drop everything after blast effect
            yield return StartCoroutine(DropAllUntilStable());
            
            FillEmptySpaces();
            
            // Wait for falling animations to complete before allowing next move
            yield return new WaitForSeconds(0.3f);

            goalPanelManager.DecrementMove();
            
            goalPanelManager.CheckWinLoseAfterMove();
            
            isProcessingMoves = false;
        }
        
        private void CreateRocketSmoke(Vector3 pos)
        {
            ParticleEffectManager.Instance.CreateRocketBlastEffect(pos, 2f);
            CameraShakeManager.Instance.ShakeForRocketBlast();
        }

        private IEnumerator RocketBlastEffect(RocketDirection dir, Vector2Int origin)
        {
            if (dir == RocketDirection.Horizontal)
            {
                // Left
                for (int x = origin.x - 1; x >= 0; x--)
                {
                    Vector3 cellPos = GetWorldPosition(x, origin.y);
                    ParticleEffectManager.Instance.CreateDirectionalBlastEffect(cellPos, Vector3.left);
                    DamageCell(new Vector2Int(x, origin.y), false, false);
                    yield return new WaitForSeconds(0.02f); // Faster blast progression
                }

                // Right
                for (int x = origin.x + 1; x < width; x++)
                {
                    Vector3 cellPos = GetWorldPosition(x, origin.y);
                    ParticleEffectManager.Instance.CreateDirectionalBlastEffect(cellPos, Vector3.right);
                    DamageCell(new Vector2Int(x, origin.y), false, false);
                    yield return new WaitForSeconds(0.02f); // Faster blast progression
                }
            }
            else
            {
                // Down
                for (int y = origin.y - 1; y >= 0; y--)
                {
                    Vector3 cellPos = GetWorldPosition(origin.x, y);
                    ParticleEffectManager.Instance.CreateDirectionalBlastEffect(cellPos, Vector3.down);
                    DamageCell(new Vector2Int(origin.x, y), false, false);
                    yield return new WaitForSeconds(0.02f); // Faster blast progression
                }

                // Up
                for (int y = origin.y + 1; y < height; y++)
                {
                    Vector3 cellPos = GetWorldPosition(origin.x, y);
                    ParticleEffectManager.Instance.CreateDirectionalBlastEffect(cellPos, Vector3.up);
                    DamageCell(new Vector2Int(origin.x, y), false, false);
                    yield return new WaitForSeconds(0.02f); // Faster blast progression
                }
            }
        }


        
        private bool DamageCell(Vector2Int pos, bool isCombo, bool isComboExplosion = false)
        {
            bool hasContent = false;
            
            // Damage cubes
            CubeBlock cube = cubes[pos.x, pos.y];
            if (cube is not null)
            {
                cubes[pos.x, pos.y] = null;
                Destroy(cube.gameObject);
                hasContent = true;
            }

            // Damage obstacles
            ObstacleBlock obs = obstacles[pos.x, pos.y];
            if (obs is not null)
            {
                bool destroyed;
                
                // Use appropriate damage type
                if (isComboExplosion)
                {
                    // 3x3 combo explosions apply 1-hit damage
                    destroyed = obs.ApplyComboRocketDamage();
                }
                else
                {
                    // Directional rocket blasts apply full rocket damage
                    destroyed = obs.ApplyRocketDamage();
                }
                
                if (destroyed)
                {
                    obstacles[pos.x, pos.y] = null;
                    goalPanelManager.DecrementObstacle(obs.obstacleType);
                }
                hasContent = true;
            }

            // Check for rockets to trigger chain reaction
            RocketBlock rocket = rockets[pos.x, pos.y];
            if (rocket is not null)
            {
                rockets[pos.x, pos.y] = null;
                // Start immediate chain reaction
                StartCoroutine(ChainReactionRocket(rocket));
                hasContent = true;
                
                // Don't stop rocket blast propagation for chain reactions
                if (!isCombo)
                    return false;
            }

            return true;
        }
        
        private IEnumerator ChainReactionRocket(RocketBlock rocket)
        {
            if (rocket == null) yield break;
            
            Debug.Log($"CHAIN REACTION: Rocket hit by another rocket's blast - directional explosion only (no 3x3)");
            
            Vector3 centerPos = GetWorldPosition(rocket.gridPosition.x, rocket.gridPosition.y);
            CreateRocketSmoke(centerPos);
            
            // Start rocket parts animation for chain reaction
            StartCoroutine(AnimateRocketParts(centerPos, rocket.direction));
            
            Destroy(rocket.gameObject);
            
            // Immediate directional blast without delay
            yield return StartCoroutine(RocketBlastEffect(rocket.direction, rocket.gridPosition));
        }

        private IEnumerator CreateSpectacularComboSequence(List<Vector3> positions, List<RocketDirection> directions)
        {
            // Calculate center point for combo
            Vector3 comboCenter = Vector3.zero;
            foreach (Vector3 pos in positions)
            {
                comboCenter += pos;
            }
            comboCenter /= positions.Count;

            // Create intersecting laser network
            yield return StartCoroutine(CreateLaserNetwork(positions, directions));

            // Spectacular combo blast at center
            ParticleEffectManager.Instance.CreateComboBlastEffect(comboCenter, positions.Count);
            
            // Secondary blasts at each rocket position
            foreach (Vector3 pos in positions)
            {
                ParticleEffectManager.Instance.CreateRocketBlastEffect(pos, 2f);
            }
        }

        private IEnumerator CreateLaserNetwork(List<Vector3> positions, List<RocketDirection> directions)
        {
            List<GameObject> networkLasers = new List<GameObject>();

            // Create lasers connecting all rockets to each other
            for (int i = 0; i < positions.Count; i++)
            {
                for (int j = i + 1; j < positions.Count; j++)
                {
                    GameObject laser = CreateNetworkLaser(positions[i], positions[j]);
                    networkLasers.Add(laser);
                }
            }

            // Keep lasers active for a brief moment
            yield return new WaitForSeconds(0.1f);

            // Intensify all lasers simultaneously
            foreach (GameObject laser in networkLasers)
            {
                if (laser != null)
                {
                    StartCoroutine(IntensifyLaser(laser));
                }
            }

            yield return new WaitForSeconds(0.1f);

            foreach (GameObject laser in networkLasers)
            {
                if (laser != null) Destroy(laser);
            }
        }

        private GameObject CreateNetworkLaser(Vector3 start, Vector3 end)
        {
            GameObject laser = new GameObject("NetworkLaser");
            SpriteRenderer sr = laser.AddComponent<SpriteRenderer>();
            
            // Load the rocket laser beam sprite
            Sprite laserSprite = Resources.Load<Sprite>("rocket_laser_beam");
            if (laserSprite == null)
            {
                // Fallback to procedural laser
                laserSprite = ParticleEffectManager.Instance.CreateCircleSprite();
            }
            
            sr.sprite = laserSprite;
            sr.color = new Color(1f, 0.3f, 0.3f, 0.7f); // Red laser for network
            sr.sortingOrder = 9;

            // Position and orient laser
            Vector3 direction = end - start;
            float distance = direction.magnitude;
            Vector3 center = (start + end) / 2f;
            
            laser.transform.position = center;
            laser.transform.localScale = new Vector3(distance, 0.8f, 1f);
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            laser.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            return laser;
        }

        private IEnumerator IntensifyLaser(GameObject laser)
        {
            SpriteRenderer sr = laser.GetComponent<SpriteRenderer>();
            if (sr == null) yield break;

            float duration = 0.1f;
            float elapsed = 0f;
            Color startColor = sr.color;
            Color endColor = new Color(1f, 1f, 0.2f, 1f);

            while (elapsed < duration && laser != null)
            {
                float t = elapsed / duration;
                sr.color = Color.Lerp(startColor, endColor, t);
                
                float scaleMultiplier = 1f + Mathf.Sin(t * 20f) * 0.2f;
                Vector3 currentScale = laser.transform.localScale;
                laser.transform.localScale = new Vector3(currentScale.x, currentScale.y * scaleMultiplier, currentScale.z);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
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
            HashSet<CubeBlock> processed = new HashSet<CubeBlock>();
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    CubeBlock cube = cubes[x, y];
                    if (cube is null || processed.Contains(cube)) continue;
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
            return new Vector3(x * (blockSize + blockSpacing), y * (blockSize + blockSpacing), 0) + gridOffset;
        }
        
        private BlockColor GetRandomColorEnum()
        {
            int value = Random.Range(0, 4);
            return (BlockColor)value; // Enum: 0=Red, 1=Green, 2=Blue, 3=Yellow
        }
        
        private Color GetCubeColor(BlockColor blockColor)
        {
            switch (blockColor)
            {
                case BlockColor.Red:
                    return new Color(1f, 0.2f, 0.2f);
                case BlockColor.Green:
                    return new Color(0.2f, 1f, 0.2f);
                case BlockColor.Blue:
                    return new Color(0.2f, 0.4f, 1f);
                case BlockColor.Yellow:
                    return new Color(1f, 1f, 0.2f);
                default:
                    return Color.white;
            }
        }
        
        private IEnumerator AnticipationEffect(Transform blockTransform)
        {
            if (blockTransform == null) yield break;
            
            Vector3 originalScale = blockTransform.localScale;
            Vector3 anticipationScale = originalScale * 1.15f;
            
            float duration = 0.08f;
            float elapsed = 0f;
            
            // Scale up quickly
            while (elapsed < duration && blockTransform != null)
            {
                float t = elapsed / duration;
                blockTransform.localScale = Vector3.Lerp(originalScale, anticipationScale, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            if (blockTransform != null)
            {
                blockTransform.localScale = anticipationScale;
            }
        }
        
        private IEnumerator AnimateRocketParts(Vector3 centerPos, RocketDirection direction)
        {
            GameObject part1 = new GameObject("RocketPart1");
            GameObject part2 = new GameObject("RocketPart2");

            SpriteRenderer sr1 = part1.AddComponent<SpriteRenderer>();
            SpriteRenderer sr2 = part2.AddComponent<SpriteRenderer>();
            Sprite partSprite1, partSprite2;

            Vector3 part1Direction, part2Direction;

            // Configure sprites and movement directions based on rocket direction
            if (direction == RocketDirection.Horizontal)
            {
                partSprite1 = Resources.Load<Sprite>("Rocket/horizontal_rocket_part_left");
                partSprite2 = Resources.Load<Sprite>("Rocket/horizontal_rocket_part_right");
                part1Direction = Vector3.left;
                part2Direction = Vector3.right;
            }
            else
            {
                partSprite1 = Resources.Load<Sprite>("Rocket/vertical_rocket_part_top");
                partSprite2 = Resources.Load<Sprite>("Rocket/vertical_rocket_part_bottom");
                part1Direction = Vector3.up;
                part2Direction = Vector3.down;
            }

            sr1.sprite = partSprite1;
            sr2.sprite = partSprite2;
            sr1.sortingOrder = 5;
            sr2.sortingOrder = 5;

            part1.transform.position = centerPos;
            part2.transform.position = centerPos;

            // Create laser beam effect between the parts
            GameObject laserBeam = ParticleEffectManager.Instance.CreateLaserBeam(centerPos, direction);

            // Start continuous trail effects for both rocket parts
            ParticleEffectManager.Instance.CreateContinuousRocketTrail(part1, part1Direction);
            ParticleEffectManager.Instance.CreateContinuousRocketTrail(part2, part2Direction);

            // Add screen shake for impact
            CameraShakeManager.Instance.ShakeForRocketBlast(1.5f);

            // Animate parts flying apart until they go outside the grid
            float speed = 8f;
            float elapsed = 0f;
            float maxDuration = 0.5f; // Maximum duration for rocket animation
            
            // Calculate closer grid boundaries for faster exit
            float gridLeft = gridOffset.x - blockSize * 0.5f;
            float gridRight = gridOffset.x + totalGridWidth + blockSize * 0.5f;
            float gridBottom = gridOffset.y - blockSize * 0.5f;
            float gridTop = gridOffset.y + totalGridHeight + blockSize * 0.5f;

            while (true)
            {
                Vector3 part1Pos = centerPos + part1Direction * speed * elapsed;
                Vector3 part2Pos = centerPos + part2Direction * speed * elapsed;
                
                part1.transform.position = part1Pos;
                part2.transform.position = part2Pos;
                
                // Check if parts are outside grid boundaries
                bool part1OutOfBounds = part1Pos.x < gridLeft || part1Pos.x > gridRight || 
                                        part1Pos.y < gridBottom || part1Pos.y > gridTop;
                bool part2OutOfBounds = part2Pos.x < gridLeft || part2Pos.x > gridRight || 
                                        part2Pos.y < gridBottom || part2Pos.y > gridTop;
                
                // If both parts are out of bounds OR max duration reached, stop the animation
                if ((part1OutOfBounds && part2OutOfBounds) || elapsed >= maxDuration)
                {
                    break;
                }
                
                // Update laser beam between the parts
                if (laserBeam != null)
                {
                    ParticleEffectManager.Instance.UpdateLaserBeam(laserBeam, part1Pos, part2Pos, direction);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Destroy parts and laser when they go off-screen
            Destroy(part1);
            Destroy(part2);
            if (laserBeam != null)
            {
                Destroy(laserBeam);
            }
        }
        
        void Update()
        {
            // Prevent input during animations
            if (isProcessingMoves) return;
            
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
                
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                if (hit.collider != null)
                {
                    var cube = hit.collider.GetComponent<CubeBlock>();
                    if (cube != null)
                    {
                        TryBlastGroupAt(cube.gridPosition);
                        return;
                    }
                    
                    var rocket = hit.collider.GetComponent<RocketBlock>();
                    if (rocket != null)
                    {
                        BlastRocketAt(rocket.gridPosition);
                        return;
                    }
                }
            }
        }
    }
}
