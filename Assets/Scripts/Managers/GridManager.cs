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
        public LevelLoader levelLoader;

        private int width;
        private int height;
        private Transform gridRoot;
        private CubeBlock[,] grid;

        void Start()
        {
            InitGrid();
        }

        void InitGrid()
        {
            LevelData data = levelLoader.levelData;
            width = data.grid_width;
            height = data.grid_height;

            gridRoot = new GameObject("GridRoot").transform;
            grid = new CubeBlock[width, height];

            // Calculate grid offset for centering
            Vector3 gridOffset = new Vector3(-width / 2f + 0.5f, -height / 2f + 0.5f, 0);

            for (int i = 0; i < data.grid.Count; i++)
            {
                string type = data.grid[i] == "rand"
                    ? GetRandomColorCode()
                    : data.grid[i];

                BlockColor color = ParseColor(type);
                int x = i % width;
                int y = i / width;
                Vector2Int pos = new Vector2Int(x, y);

                Vector3 worldPos = new Vector3(x, y, 0) + gridOffset;

                CreateCube(color, pos, worldPos);
            }
        }

        void CreateCube(BlockColor color, Vector2Int gridPos, Vector3 worldPos)
        {
            GameObject obj = Instantiate(cubePrefab, worldPos, Quaternion.identity, gridRoot);
            CubeBlock cube = obj.GetComponent<CubeBlock>();
            cube.Initialize(color, gridPos);
            grid[gridPos.x, gridPos.y] = cube;
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
