using UnityEngine;

namespace Blocks
{
    public class CubeBlock : MonoBehaviour
    {
        public Vector2Int GridPos { get; set; }
        public BlockColor ColorType;

        private SpriteRenderer spriteRenderer;

        public void Initialize(BlockColor color, Vector2Int gridPos)
        {
            ColorType = color;
            GridPos = gridPos;
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetColorSprite();
        }

        private void SetColorSprite()
        {
            string spritePath = $"Cubes/DefaultState/{ColorType.ToString().ToLower()}";
            Sprite sprite = Resources.Load<Sprite>(spritePath);
            if (sprite != null)
            {
                spriteRenderer.sprite = sprite;
            }
            else
            {
                Debug.LogError($"Missing sprite at path: Resources/{spritePath}.png");
            }
        }
    }
}