using UnityEngine;

namespace Blocks
{
    public class ObstacleBlock : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        public ObstacleType obstacleType;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(ObstacleType type)
        {
            this.obstacleType = type;

            string path = type switch
            {
                ObstacleType.Box => "Obstacles/Box/box",
                ObstacleType.Stone => "Obstacles/Stone/stone",
                ObstacleType.Vase => "Obstacles/Vase/vase_01", // Or random vase_01 / vase_02 later
                _ => ""
            };

            Sprite sprite = Resources.Load<Sprite>(path);
            if (sprite != null)
                spriteRenderer.sprite = sprite;
            else
                Debug.LogError($"Missing obstacle sprite at path: {path}.png");
        }
    }
}