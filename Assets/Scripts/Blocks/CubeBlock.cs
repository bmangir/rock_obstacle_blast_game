using Managers;
using UnityEngine;

namespace Blocks
{
    public class CubeBlock : MonoBehaviour
    {
        public BlockColor color;
        public Vector2Int gridPosition;
        private GameObject rocketHint;
        private GridManager gridManager;

        private void Start()
        {
            gridManager = FindObjectOfType<GridManager>();
            CreateRocketHint();
        }

        public void Initialize(BlockColor color, Vector2Int pos)
        {
            this.color = color;
            this.gridPosition = pos;

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>($"Cubes/DefaultState/{color.ToString().ToLower()}");
        }
        
        private void CreateRocketHint()
        {
            rocketHint = new GameObject("RocketHint");
            rocketHint.transform.SetParent(transform);
            rocketHint.transform.localPosition = Vector3.zero;
            
            SpriteRenderer sr = rocketHint.AddComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>($"Cubes/RocketState/{color.ToString().ToLower()}_rocket");
            sr.sortingOrder = 1;
            rocketHint.SetActive(false);
        }
        
        public void ShowRocketHint()
        {
            if (rocketHint is not null)
                rocketHint.SetActive(true);
        }
        
        public void HideRocketHint()
        {
            if (rocketHint is not null)
                rocketHint.SetActive(false);
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