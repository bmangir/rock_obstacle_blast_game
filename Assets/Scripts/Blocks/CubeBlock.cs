using Managers;
using UnityEngine;

namespace Blocks
{
    public class CubeBlock : MonoBehaviour
    {
        public BlockColor color;
        public Vector2Int gridPosition;

        private GridManager gridManager;

        private void Start()
        {
            gridManager = FindObjectOfType<GridManager>();
        }

        public void Initialize(BlockColor color, Vector2Int pos)
        {
            this.color = color;
            this.gridPosition = pos;

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>($"Cubes/DefaultState/{color.ToString().ToLower()}");
        }

        private void OnMouseDown()
        {
            if (gridManager != null)
            {
                gridManager.TryBlastGroupAt(gridPosition);
            }
        }
    }
}