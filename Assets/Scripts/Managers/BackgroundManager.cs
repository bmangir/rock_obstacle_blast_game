using UnityEngine;

namespace Managers
{
    public class BackgroundManager : MonoBehaviour
    {
        [SerializeField] private string backgroundPath = "Menu/background";
        [SerializeField] private int sortingOrder = -10;

        void Start()
        {
            Sprite sprite = Resources.Load<Sprite>(backgroundPath);
            if (sprite == null)
            {
                Debug.LogError($"Background not found at Resources/{backgroundPath}.png");
                return;
            }

            SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = sortingOrder;

            FitSpriteToScreen(sr);
        }

        private void FitSpriteToScreen(SpriteRenderer sr)
        {
            Camera cam = Camera.main;

            float screenHeight = 2f * cam.orthographicSize;
            float screenWidth = screenHeight * cam.aspect;

            float spriteHeight = sr.sprite.bounds.size.y;
            float spriteWidth = sr.sprite.bounds.size.x;

            float scaleX = screenWidth / spriteWidth;
            float scaleY = screenHeight / spriteHeight;

            float scale = Mathf.Max(scaleX, scaleY); 

            transform.localScale = new Vector3(scale, scale, 1f);
            transform.position = new Vector3(0, 0, 10); // send to back
        }
    }
}