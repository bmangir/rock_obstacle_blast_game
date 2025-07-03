using UnityEngine;

namespace Factories
{
    public class ClickDebugger : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null)
                {
                    Debug.Log("Clicked on: " + hit.collider.name);
                }
            }
        }
    }
}