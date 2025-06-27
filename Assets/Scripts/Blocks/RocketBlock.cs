using UnityEngine;

namespace Blocks
{
    public class RocketBlock : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        public RocketDirection direction;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(RocketDirection dir)
        {
            direction = dir;

            string path = direction switch
            {
                RocketDirection.Horizontal => "Rocket/horizontal_rocket",
                RocketDirection.Vertical => "Rocket/vertical_rocket",
                _ => "Rocket/horizontal_rocket"
            };

            Sprite sprite = Resources.Load<Sprite>(path);

            if (sprite != null)
                spriteRenderer.sprite = sprite;
            else
                Debug.LogError($"Rocket sprite not found at Resources/{path}.png");
        }
    }
}