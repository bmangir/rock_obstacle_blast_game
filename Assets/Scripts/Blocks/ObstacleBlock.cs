using UnityEngine;
using Managers;

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
                CreateDestroyEffect();
                Destroy(gameObject);
                return true;
            }

            return false;
        }
        
        public bool CanFall()
        {
            return obstacleType == ObstacleType.Vase;
        }
        
        private void CreateDestroyEffect()
        {
            string obstacleTypeString = obstacleType.ToString();
            ParticleEffectManager.Instance.CreateObstacleDestroyEffect(transform.position, obstacleTypeString);
        }
        
        public bool ApplyRocketDamage()
        {
            switch (obstacleType)
            {
                case ObstacleType.Box:
                    CreateDestroyEffect();
                    Destroy(gameObject);
                    return true;
                    
                case ObstacleType.Stone:
                    CreateDestroyEffect();
                    Destroy(gameObject);
                    return true;
                    
                case ObstacleType.Vase:
                    health--;
                    if (health == 1)
                    {
                        spriteRenderer.sprite = Resources.Load<Sprite>("Obstacles/Vase/vase_02");
                    }
                    else if (health <= 0)
                    {
                        CreateDestroyEffect();
                        Destroy(gameObject);
                        return true;
                    }
                    return false;
                    
                default:
                    return false;
            }
        }
        
        /*
         * Apply 1-hit damage for adjacent rocket combo 3x3 explosions
         * This is similar to ApplyBlastDamage but works for all obstacle types
         */
        public bool ApplyComboRocketDamage()
        {
            health--;

            // Change view of vase when it takes 1 hit
            if (obstacleType == ObstacleType.Vase && health == 1)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Obstacles/Vase/vase_02");
            }

            if (health <= 0)
            {
                CreateDestroyEffect();
                Destroy(gameObject);
                return true;
            }

            return false;
        }
    }
}