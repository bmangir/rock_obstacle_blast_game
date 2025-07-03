using UnityEngine;
using System.Collections;

namespace Managers
{
    public class CameraShakeManager : MonoBehaviour
    {
        private static CameraShakeManager instance;
        public static CameraShakeManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<CameraShakeManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("CameraShakeManager");
                        instance = go.AddComponent<CameraShakeManager>();
                    }
                }
                return instance;
            }
        }

        private Camera mainCamera;
        private Vector3 originalPosition;
        private Coroutine shakeCoroutine;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeCamera();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializeCamera()
        {
            mainCamera = Camera.main;
            if (mainCamera != null)
            {
                originalPosition = mainCamera.transform.position;
            }
        }

        public void ShakeForCubeBlast(int cubeCount)
        {
            // Shake intensity based on number of cubes blasted
            float intensity = Mathf.Clamp(cubeCount * 0.03f, 0.02f, 0.15f);
            float duration = Mathf.Clamp(cubeCount * 0.05f, 0.1f, 0.3f);
            StartShake(intensity, duration);
        }

        public void ShakeForRocketBlast(float intensity = 1f)
        {
            // More intense shake for rockets
            float shakeIntensity = 0.1f * intensity;
            float duration = 0.25f * intensity;
            StartShake(shakeIntensity, duration);
        }

        public void ShakeForComboBlast(int rocketCount)
        {
            // Very intense shake for rocket combos
            float intensity = Mathf.Clamp(rocketCount * 0.08f, 0.15f, 0.3f);
            float duration = Mathf.Clamp(rocketCount * 0.1f, 0.3f, 0.6f);
            StartShake(intensity, duration);
        }

        private void StartShake(float intensity, float duration)
        {
            if (mainCamera == null)
            {
                InitializeCamera();
                if (mainCamera == null) return;
            }

            // Stop any existing shake
            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
            }

            shakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
        }

        private IEnumerator ShakeCoroutine(float intensity, float duration)
        {
            Vector3 startPosition = mainCamera.transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * intensity;
                float y = Random.Range(-1f, 1f) * intensity;

                // Reduce intensity over time
                float currentIntensity = intensity * (1f - (elapsed / duration));
                x *= currentIntensity;
                y *= currentIntensity;

                mainCamera.transform.position = new Vector3(
                    startPosition.x + x,
                    startPosition.y + y,
                    startPosition.z
                );

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Return to original position smoothly
            yield return StartCoroutine(ReturnToOriginalPosition(startPosition, 0.1f));
            shakeCoroutine = null;
        }

        private IEnumerator ReturnToOriginalPosition(Vector3 targetPosition, float duration)
        {
            Vector3 startPosition = mainCamera.transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                t = Mathf.SmoothStep(0f, 1f, t); // Smooth easing

                mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                elapsed += Time.deltaTime;
                yield return null;
            }

            mainCamera.transform.position = targetPosition;
        }

        // Update original position when camera moves (for level transitions, etc.)
        public void UpdateOriginalPosition()
        {
            if (mainCamera != null)
            {
                originalPosition = mainCamera.transform.position;
            }
        }
    }
} 