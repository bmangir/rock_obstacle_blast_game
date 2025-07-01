using Managers;
using UnityEngine;

namespace Blocks
{
    public class RocketBlock : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        public RocketDirection direction;
        private GridManager gridManager;
        public Vector2Int gridPosition;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            // Ensure have a collider
            if (GetComponent<Collider2D>() == null)
            {
                gameObject.AddComponent<BoxCollider2D>();
                Debug.Log("Added BoxCollider2D to rocket");
            }
            
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                0
            );
        }
        
        private void Start()
        {
            gridManager = FindObjectOfType<GridManager>();
            
            if (gridManager == null)
            {
                Debug.LogError("GridManager not found in scene!");
            }
            
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.size = spriteRenderer.sprite.bounds.size;
            }
        }

        public void Initialize(RocketDirection dir, Vector2Int gridPos)
        {
            direction = dir;
            gridPosition = gridPos;
            
            LoadRocketSprite(dir);
        }
        
        private void LoadRocketSprite(RocketDirection dir)
        {
            string variant = dir == RocketDirection.Horizontal ? "horizontal_rocket" : "vertical_rocket";
            string path = $"Rocket/{variant}";
            Sprite sprite = Resources.Load<Sprite>(path);
            if (sprite is not null)
            {
                spriteRenderer.sprite = sprite;
            }
            else
            {
                Debug.LogError($"Rocket sprite not found at {path}");
            }
        }
        
        private string GetRandomRocketVariant(RocketDirection dir)
        {
            if (dir == RocketDirection.Horizontal)
            {
                int rnd = Random.Range(0, 3);
                return rnd switch {
                    0 => "horizontal_rocket",
                    1 => "horizontal_rocket_part_left",
                    _ => "horizontal_rocket_part_right"
                };
            }
            else
            {
                int rnd = Random.Range(0, 3);
                return rnd switch {
                    0 => "vertical_rocket",
                    1 => "vertical_rocket_part_bottom",
                    _ => "vertical_rocket_part_top"
                };
            }
        }
        
        private void OnMouseDown()
        {
            if (gridManager == null)
            {
                gridManager = FindObjectOfType<GridManager>();
                
                if (gridManager == null)
                {
                    Debug.LogError("GridManager reference is still null!");
                    return;
                }
            }
            gridManager.BlastRocketAt(gridPosition);
        }
    }
}