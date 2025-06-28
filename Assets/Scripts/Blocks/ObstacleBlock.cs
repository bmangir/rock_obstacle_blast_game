using UnityEngine;

namespace Blocks
{
    public class ObstacleBlock : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        public ObstacleType obstacleType;
        public Vector2Int gridPosition;
        private int health;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /*
         * Initialize health of obstacles and images
         */
        public void Initialize(ObstacleType type, Vector2Int pos)
        {
            this.obstacleType = type;
            gridPosition = pos;

            switch (type)
            {
                case ObstacleType.Box:
                    health = 1;
                    spriteRenderer.sprite = Resources.Load<Sprite>("Obstacles/Box/box");
                    break;
                case ObstacleType.Stone:
                    health = 1;
                    spriteRenderer.sprite = Resources.Load<Sprite>("Obstacles/Stone/stone");
                    break;
                case ObstacleType.Vase:
                    health = 2;
                    spriteRenderer.sprite = Resources.Load<Sprite>("Obstacles/Vase/vase_01");
                    break;
            }
        }
        
        /*
         * Apply damages to destroy obstacles
         */
        public bool ApplyBlastDamage()
        {
            if (obstacleType == ObstacleType.Stone)
                return false; // not damaged by cube blast

            health--;

            // Change view of vase when it is gotten 1 hit
            if (obstacleType == ObstacleType.Vase && health == 1)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Obstacles/Vase/vase_02");
            }

            if (health <= 0)
            {
                Destroy(gameObject);
                return true;
            }

            return false;
        }
        
        public bool CanFall()
        {
            return obstacleType == ObstacleType.Vase;
        }
    }
}