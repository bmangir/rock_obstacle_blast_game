using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Blocks;

namespace Managers
{
    public class ParticleEffectManager : MonoBehaviour
    {
        private static ParticleEffectManager instance;
        public static ParticleEffectManager Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = FindObjectOfType<ParticleEffectManager>();
                    if (instance is null)
                    {
                        GameObject go = new GameObject("ParticleEffectManager");
                        instance = go.AddComponent<ParticleEffectManager>();
                    }
                }
                return instance;
            }
        }

        [System.Serializable]
        public class ParticleSettings
        {
            [Header("Particle Count")]
            public int minParticles = 6;
            public int maxParticles = 12;
            
            [Header("Velocity")]
            public float minVelocity = 2f;
            public float maxVelocity = 5f;
            public float gravity = -9.8f;
            
            [Header("Scale")]
            public float minStartScale = 0.3f;
            public float maxStartScale = 0.6f;
            public float minEndScale = 0.1f;
            public float maxEndScale = 0.3f;
            
            [Header("Rotation")]
            public float minRotationSpeed = 90f;
            public float maxRotationSpeed = 360f;
            
            [Header("Timing")]
            public float minLifetime = 0.8f;
            public float maxLifetime = 1.2f;
            public float fadeStartTime = 0.3f;
            
            [Header("Visual")]
            public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
            public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        }

        [SerializeField] private ParticleSettings cubeBlastSettings;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDefaultSettings();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializeDefaultSettings()
        {
            if (cubeBlastSettings == null)
            {
                cubeBlastSettings = new ParticleSettings
                {
                    minParticles = 8,
                    maxParticles = 16,
                    minVelocity = 3f,
                    maxVelocity = 6f,
                    gravity = -12f,
                    minStartScale = 0.15f, 
                    maxStartScale = 0.35f,
                    minEndScale = 0.05f,
                    maxEndScale = 0.15f,
                    minRotationSpeed = 180f,
                    maxRotationSpeed = 540f,
                    minLifetime = 0.6f,
                    maxLifetime = 1.0f,
                    fadeStartTime = 0.2f,
                    scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0),
                    alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0)
                };
            }
        }

        public void CreateCubeBlastEffect(Vector3 position, Color cubeColor, string colorName)
        {
            StartCoroutine(SpawnParticleEffect(position, cubeColor, colorName, cubeBlastSettings));
            CreateBurstEffect(position, cubeColor);
        }

        public void CreateRocketBlastEffect(Vector3 position, float intensity = 1f)
        {
            ParticleSettings rocketSettings = new ParticleSettings
            {
                minParticles = Mathf.RoundToInt(cubeBlastSettings.minParticles * intensity),
                maxParticles = Mathf.RoundToInt(cubeBlastSettings.maxParticles * intensity),
                minVelocity = cubeBlastSettings.minVelocity * 1.5f,
                maxVelocity = cubeBlastSettings.maxVelocity * 1.5f,
                gravity = cubeBlastSettings.gravity,
                minStartScale = cubeBlastSettings.minStartScale * 0.8f,
                maxStartScale = cubeBlastSettings.maxStartScale * 0.8f,
                minEndScale = cubeBlastSettings.minEndScale,
                maxEndScale = cubeBlastSettings.maxEndScale,
                minRotationSpeed = cubeBlastSettings.minRotationSpeed * 1.2f,
                maxRotationSpeed = cubeBlastSettings.maxRotationSpeed * 1.2f,
                minLifetime = cubeBlastSettings.minLifetime * 1.3f,
                maxLifetime = cubeBlastSettings.maxLifetime * 1.3f,
                fadeStartTime = cubeBlastSettings.fadeStartTime,
                scaleCurve = cubeBlastSettings.scaleCurve,
                alphaCurve = cubeBlastSettings.alphaCurve
            };

            StartCoroutine(SpawnRocketParticleEffect(position, rocketSettings));
            Color rocketBlastColor = new Color(1f, 0.9f, 0.2f); // Golden yellow
            CreateBurstEffect(position, rocketBlastColor, intensity * 1.5f);
        }

        public void CreateObstacleDestroyEffect(Vector3 position, string obstacleType)
        {
            ParticleSettings obstacleSettings = new ParticleSettings
            {
                minParticles = 4,
                maxParticles = 8,
                minVelocity = cubeBlastSettings.minVelocity * 0.8f,
                maxVelocity = cubeBlastSettings.maxVelocity * 0.8f,
                gravity = cubeBlastSettings.gravity,
                minStartScale = 0.2f,
                maxStartScale = 0.4f,
                minEndScale = 0.1f,
                maxEndScale = 0.2f,
                minRotationSpeed = cubeBlastSettings.minRotationSpeed,
                maxRotationSpeed = cubeBlastSettings.maxRotationSpeed,
                minLifetime = 0.8f,
                maxLifetime = 1.4f,
                fadeStartTime = cubeBlastSettings.fadeStartTime,
                scaleCurve = cubeBlastSettings.scaleCurve,
                alphaCurve = cubeBlastSettings.alphaCurve
            };

            StartCoroutine(SpawnObstacleParticleEffect(position, obstacleType, obstacleSettings));
        }

        public void CreateDirectionalBlastEffect(Vector3 position, Vector3 direction)
        {
            StartCoroutine(SpawnDirectionalBlastEffect(position, direction));
            
            Color blastColor = new Color(1f, 0.9f, 0.2f); // Golden yellow
            CreateBurstEffect(position, blastColor, 0.8f);
        }

        public GameObject CreateLaserBeam(Vector3 center, RocketDirection direction)
        {
            GameObject laserBeam = new GameObject("LaserBeam");
            laserBeam.transform.position = center;

            SpriteRenderer sr = laserBeam.AddComponent<SpriteRenderer>();
            
            // Create a laser beam sprite
            Sprite laserSprite = CreateLaserSprite(direction);
            sr.sprite = laserSprite;
            sr.color = new Color(1f, 0.9f, 0.2f, 0f);
            sr.sortingOrder = 8; // Between parts and particles

            // Start laser beam animation
            StartCoroutine(AnimateLaserBeamAppearance(laserBeam));

            return laserBeam;
        }

        public void UpdateLaserBeam(GameObject laserBeam, Vector3 part1Pos, Vector3 part2Pos, RocketDirection direction)
        {
            if (laserBeam is null) return;

            Vector3 center = (part1Pos + part2Pos) / 2f;
            float distance = Vector3.Distance(part1Pos, part2Pos);

            laserBeam.transform.position = center;

            // Scale the laser beam based on distance between parts
            if (direction == RocketDirection.Horizontal)
            {
                laserBeam.transform.localScale = new Vector3(distance, 3f, 1f); 
            }
            else
            {
                laserBeam.transform.localScale = new Vector3(3f, distance, 1f);
            }

            SpriteRenderer sr = laserBeam.GetComponent<SpriteRenderer>();
            if (sr is not null)
            {
                float pulse = Mathf.Sin(Time.time * 15f) * 0.3f + 0.95f; 
                Color color = new Color(1f, 0.9f, 0.2f, pulse);
                sr.color = color;
                
                float scalePulse = Mathf.Sin(Time.time * 12f) * 0.1f + 1f;
                Vector3 currentScale = laserBeam.transform.localScale;
                if (direction == RocketDirection.Horizontal)
                {
                    laserBeam.transform.localScale = new Vector3(currentScale.x, currentScale.y * scalePulse, currentScale.z);
                }
                else
                {
                    laserBeam.transform.localScale = new Vector3(currentScale.x * scalePulse, currentScale.y, currentScale.z);
                }
            }
        }

        public void CreateRocketTrailEffect(Vector3 position, Vector3 direction)
        {
            StartCoroutine(SpawnRocketTrailEffect(position, direction));
        }

        public void CreateContinuousRocketTrail(GameObject rocketPart, Vector3 direction)
        {
            StartCoroutine(ContinuousRocketTrailEffect(rocketPart, direction));
        }

        public void CreateComboBlastEffect(Vector3 position, float intensity = 2f)
        {
            // Start the spectacular combo sequence
            StartCoroutine(SpectacularComboExplosion(position, intensity));
        }

        private IEnumerator SpectacularComboExplosion(Vector3 centerPosition, float intensity)
        {
            // Energy Buildup (0.1s)
            StartCoroutine(CreateEnergyBuildupEffect(centerPosition));
            yield return new WaitForSeconds(0.1f);

            // Screen Flash + Massive Burst (0.05s)
            StartCoroutine(CreateScreenFlashEffect());
            CreateMassiveComboBlast(centerPosition, intensity);
            yield return new WaitForSeconds(0.05f);

            // Multiple Shock-waves (0.2s)
            StartCoroutine(CreateMultipleShockwaves(centerPosition, intensity));
            yield return new WaitForSeconds(0.2f);
        }

        private IEnumerator CreateEnergyBuildupEffect(Vector3 position)
        {
            List<GameObject> energyOrbs = new List<GameObject>();
            int orbCount = 8;
            
            for (int i = 0; i < orbCount; i++)
            {
                GameObject orb = new GameObject("EnergyOrb");
                SpriteRenderer sr = orb.AddComponent<SpriteRenderer>();
                
                // Use star particle for energy orbs
                Sprite starSprite = Resources.Load<Sprite>("Rocket/Particles/particle_star");
                sr.sprite = starSprite;
                sr.color = new Color(1f, 1f, 0.3f, 0.8f); // Bright energy color
                sr.sortingOrder = 15;
                
                float angle = (360f / orbCount) * i * Mathf.Deg2Rad;
                Vector3 startPos = position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 2f;
                orb.transform.position = startPos;
                orb.transform.localScale = Vector3.one * 0.3f;
                
                energyOrbs.Add(orb);
            }

            // Animate orbs spiraling inward while growing
            float duration = 0.1f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                
                for (int i = 0; i < energyOrbs.Count; i++)
                {
                    if (energyOrbs[i] is not null)
                    {
                        float angle = (360f / orbCount) * i * Mathf.Deg2Rad + t * 720f * Mathf.Deg2Rad; // Spiral
                        float radius = Mathf.Lerp(2f, 0.2f, t);
                        Vector3 newPos = position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
                        energyOrbs[i].transform.position = newPos;
                        
                        // Grow and brighten
                        float scale = Mathf.Lerp(0.3f, 0.6f, t);
                        energyOrbs[i].transform.localScale = Vector3.one * scale;
                        
                        SpriteRenderer sr = energyOrbs[i].GetComponent<SpriteRenderer>();
                        float alpha = Mathf.Lerp(0.8f, 1f, t);
                        sr.color = new Color(1f, 1f, 0.3f, alpha);
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }

            foreach (GameObject orb in energyOrbs)
            {
                if (orb is not null) Destroy(orb);
            }
        }

        private IEnumerator CreateScreenFlashEffect()
        {
            // Create full screen white flash
            GameObject flash = new GameObject("ScreenFlash");
            SpriteRenderer sr = flash.AddComponent<SpriteRenderer>();
            
            // Create a large white square that covers the screen
            Texture2D flashTexture = new Texture2D(1, 1);
            flashTexture.SetPixel(0, 0, Color.white);
            flashTexture.Apply();
            
            Sprite flashSprite = Sprite.Create(flashTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            sr.sprite = flashSprite;
            sr.color = new Color(1f, 1f, 1f, 0.8f);
            sr.sortingOrder = 100;
            
            // Position and scale to cover camera view
            Camera mainCam = Camera.main;
            flash.transform.position = mainCam.transform.position + Vector3.forward * (mainCam.nearClipPlane + 0.1f);
            
            float camHeight = 2f * mainCam.orthographicSize;
            float camWidth = camHeight * mainCam.aspect;
            flash.transform.localScale = new Vector3(camWidth * 2f, camHeight * 2f, 1f);

            // Flash animation
            float duration = 0.15f;
            float elapsed = 0f;
            
            while (elapsed < duration && flash != null)
            {
                float t = elapsed / duration;
                float alpha = Mathf.Lerp(0.8f, 0f, t * t);
                sr.color = new Color(1f, 1f, 1f, alpha);
                
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (flash is not null) Destroy(flash);
        }

        private void CreateMassiveComboBlast(Vector3 position, float intensity)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 offset = (Vector3)Random.insideUnitCircle * 0.5f;
                
                ParticleSettings comboSettings = new ParticleSettings
                {
                    minParticles = Mathf.RoundToInt(cubeBlastSettings.minParticles * intensity * 2f),
                    maxParticles = Mathf.RoundToInt(cubeBlastSettings.maxParticles * intensity * 2f),
                    minVelocity = cubeBlastSettings.minVelocity * 3f,
                    maxVelocity = cubeBlastSettings.maxVelocity * 3f,
                    gravity = cubeBlastSettings.gravity * 0.5f,
                    minStartScale = cubeBlastSettings.minStartScale * 1.5f,
                    maxStartScale = cubeBlastSettings.maxStartScale * 2f,
                    minEndScale = cubeBlastSettings.minEndScale,
                    maxEndScale = cubeBlastSettings.maxEndScale * 1.5f,
                    minRotationSpeed = cubeBlastSettings.minRotationSpeed * 2f,
                    maxRotationSpeed = cubeBlastSettings.maxRotationSpeed * 2f,
                    minLifetime = cubeBlastSettings.minLifetime * 1.5f,
                    maxLifetime = cubeBlastSettings.maxLifetime * 1.5f,
                    fadeStartTime = cubeBlastSettings.fadeStartTime,
                    scaleCurve = cubeBlastSettings.scaleCurve,
                    alphaCurve = cubeBlastSettings.alphaCurve
                };

                StartCoroutine(SpawnRocketParticleEffect(position + offset, comboSettings));
            }
            
            Color[] burstColors = {
                new Color(1f, 0.9f, 0.1f), // Bright golden
                new Color(1f, 0.6f, 0.1f), // Orange
                new Color(1f, 1f, 0.3f)    // Yellow-white
            };
            
            foreach (Color color in burstColors)
            {
                CreateBurstEffect(position, color, intensity * 3f);
            }
        }

        private IEnumerator CreateMultipleShockwaves(Vector3 center, float intensity)
        {
            for (int wave = 0; wave < 3; wave++)
            {
                StartCoroutine(CreateShockwaveRing(center, wave * 0.05f, intensity));
                yield return new WaitForSeconds(0.05f);
            }
        }

        private IEnumerator CreateShockwaveRing(Vector3 center, float delay, float intensity)
        {
            yield return new WaitForSeconds(delay);
            
            GameObject ring = new GameObject("ShockwaveRing");
            SpriteRenderer sr = ring.AddComponent<SpriteRenderer>();
            
            sr.sprite = CreateCircleSprite();
            sr.color = new Color(1f, 0.8f, 0.2f, 0.6f);
            sr.sortingOrder = 12;
            
            ring.transform.position = center;
            ring.transform.localScale = Vector3.zero;

            float duration = 0.8f;
            float elapsed = 0f;
            float maxScale = 8f * intensity;
            
            while (elapsed < duration && ring != null)
            {
                float t = elapsed / duration;
                
                float currentScale = Mathf.Lerp(0f, maxScale, t);
                ring.transform.localScale = Vector3.one * currentScale;
                
                float alpha = Mathf.Lerp(0.6f, 0f, t * t);
                sr.color = new Color(1f, 0.8f, 0.2f, alpha);
                
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (ring is not null) Destroy(ring);
        }

        private IEnumerator AnimateLaserBeamAppearance(GameObject laserBeam)
        {
            if (laserBeam is null) yield break;

            SpriteRenderer sr = laserBeam.GetComponent<SpriteRenderer>();
            if (sr is null) yield break;

            // Animate laser beam appearing with a crackling effect
            float appearDuration = 0.2f;
            float elapsed = 0f;

            while (elapsed < appearDuration && laserBeam != null)
            {
                float t = elapsed / appearDuration;
                
                // Flash effect during appearance
                float flash = Mathf.Sin(t * 30f) * 0.3f + 0.7f;
                float alpha = Mathf.Lerp(0f, 0.95f, t) * flash;
                
                Color color = new Color(1f, 0.9f, 0.2f, alpha);
                sr.color = color;

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Set final color
            if (laserBeam is not null && sr is not null)
            {
                sr.color = new Color(1f, 0.9f, 0.2f, 0.95f);
            }
        }

        private IEnumerator ContinuousRocketTrailEffect(GameObject rocketPart, Vector3 direction)
        {
            if (rocketPart is null) yield break;

            float trailInterval = 0.12f; // Reduced frequency for faster animation
            float lastTrailTime = 0f;
            float startDelay = 0.05f; // Smaller delay for faster response
            float elapsed = 0f;

            while (rocketPart != null)
            {
                elapsed += Time.deltaTime;
                
                if (elapsed > startDelay && Time.time - lastTrailTime >= trailInterval)
                {
                    // Calculate trail spawn position (behind the rocket)
                    Vector3 trailOffset = -direction * 0.4f; 
                    Vector3 trailPosition = rocketPart.transform.position + trailOffset;
                    
                    CreateRocketTrailEffect(trailPosition, direction);
                    lastTrailTime = Time.time;
                }

                yield return null;
            }
        }

        private IEnumerator SpawnParticleEffect(Vector3 center, Color color, string colorName, ParticleSettings settings)
        {
            // Load the particle sprite
            Sprite particleSprite = Resources.Load<Sprite>($"Cubes/Particles/particle_{colorName}");
            if (particleSprite is null)
            {
                Debug.LogWarning($"Could not load particle sprite for color: {colorName}");
                yield break;
            }

            int particleCount = Random.Range(settings.minParticles, settings.maxParticles + 1);
            List<GameObject> particles = new List<GameObject>();

            // Create all particles
            for (int i = 0; i < particleCount; i++)
            {
                GameObject particle = CreateSingleParticle(center, particleSprite, color, settings);
                particles.Add(particle);
            }

            // Animate all particles
            List<Coroutine> animations = new List<Coroutine>();
            foreach (GameObject particle in particles)
            {
                if (particle is not null)
                {
                    animations.Add(StartCoroutine(AnimateParticle(particle, settings)));
                }
            }

            // Wait for all animations to complete
            foreach (Coroutine animation in animations)
            {
                if (animation != null)
                {
                    yield return animation;
                }
            }
        }

        private IEnumerator SpawnRocketParticleEffect(Vector3 center, ParticleSettings settings)
        {
            // Use both smoke and star particles for rockets
            string[] rocketParticles = { "Rocket/Particles/particle_smoke", "Rocket/Particles/particle_star" };
            List<GameObject> particles = new List<GameObject>();

            int particleCount = Random.Range(settings.minParticles, settings.maxParticles + 1);
            
            for (int i = 0; i < particleCount; i++)
            {
                string particlePath = rocketParticles[Random.Range(0, rocketParticles.Length)];
                Sprite particleSprite = Resources.Load<Sprite>(particlePath);
                
                if (particleSprite is not null)
                {
                    Color rocketParticleColor = new Color(1f, 0.9f, 0.3f);
                    GameObject particle = CreateSingleParticle(center, particleSprite, rocketParticleColor, settings);
                    particles.Add(particle);
                }
            }

            List<Coroutine> animations = new List<Coroutine>();
            foreach (GameObject particle in particles)
            {
                if (particle is not null)
                {
                    animations.Add(StartCoroutine(AnimateParticle(particle, settings)));
                }
            }

            foreach (Coroutine animation in animations)
            {
                if (animation != null)
                {
                    yield return animation;
                }
            }
        }

        private IEnumerator SpawnObstacleParticleEffect(Vector3 center, string obstacleType, ParticleSettings settings)
        {
            // Get the appropriate particle sprites for this obstacle type
            string[] particlePaths = GetObstacleParticlePaths(obstacleType);
            List<GameObject> particles = new List<GameObject>();

            int particleCount = Random.Range(settings.minParticles, settings.maxParticles + 1);

            for (int i = 0; i < particleCount; i++)
            {
                string particlePath = particlePaths[Random.Range(0, particlePaths.Length)];
                Sprite particleSprite = Resources.Load<Sprite>(particlePath);
                
                if (particleSprite is not null)
                {
                    GameObject particle = CreateSingleParticle(center, particleSprite, Color.white, settings);
                    particles.Add(particle);
                }
            }

            List<Coroutine> animations = new List<Coroutine>();
            foreach (GameObject particle in particles)
            {
                if (particle is not null)
                {
                    animations.Add(StartCoroutine(AnimateParticle(particle, settings)));
                }
            }

            foreach (Coroutine animation in animations)
            {
                if (animation != null)
                {
                    yield return animation;
                }
            }
        }

        private IEnumerator SpawnDirectionalBlastEffect(Vector3 position, Vector3 direction)
        {
            string[] rocketParticles = { "Rocket/Particles/particle_smoke", "Rocket/Particles/particle_star" };
            
            ParticleSettings directionalSettings = new ParticleSettings
            {
                minParticles = 3,
                maxParticles = 6,
                minVelocity = 4f,
                maxVelocity = 7f,
                gravity = -5f,
                minStartScale = 0.1f,
                maxStartScale = 0.25f,
                minEndScale = 0.05f,
                maxEndScale = 0.15f,
                minRotationSpeed = 200f,
                maxRotationSpeed = 400f,
                minLifetime = 0.4f,
                maxLifetime = 0.7f,
                fadeStartTime = 0.1f,
                scaleCurve = cubeBlastSettings.scaleCurve,
                alphaCurve = cubeBlastSettings.alphaCurve
            };

            Color blastColor = new Color(1f, 0.9f, 0.3f); // Golden yellow
            List<GameObject> particles = new List<GameObject>();

            int particleCount = Random.Range(directionalSettings.minParticles, directionalSettings.maxParticles + 1);

            for (int i = 0; i < particleCount; i++)
            {
                string particlePath = rocketParticles[Random.Range(0, rocketParticles.Length)];
                Sprite particleSprite = Resources.Load<Sprite>(particlePath);
                
                if (particleSprite is not null)
                {
                    GameObject particle = CreateDirectionalParticle(position, particleSprite, blastColor, direction, directionalSettings);
                    particles.Add(particle);
                }
            }

            List<Coroutine> animations = new List<Coroutine>();
            foreach (GameObject particle in particles)
            {
                if (particle is not null)
                {
                    animations.Add(StartCoroutine(AnimateParticle(particle, directionalSettings)));
                }
            }

            foreach (Coroutine animation in animations)
            {
                if (animation != null)
                {
                    yield return animation;
                }
            }
        }

        private GameObject CreateDirectionalParticle(Vector3 center, Sprite sprite, Color color, Vector3 direction, ParticleSettings settings)
        {
            GameObject particle = new GameObject("DirectionalParticle");
            particle.transform.position = center;

            SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            sr.sortingOrder = 10;

            // Random initial scale
            float startScale = Random.Range(settings.minStartScale, settings.maxStartScale);
            particle.transform.localScale = Vector3.one * startScale;

            // Store initial values in a component
            ParticleData data = particle.AddComponent<ParticleData>();
            
            // Bias velocity towards the blast direction but add some randomness
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector2 blastDirection = new Vector2(direction.x, direction.y);
            Vector2 finalDirection = (blastDirection * 0.7f + randomDirection * 0.3f).normalized;
            
            data.velocity = finalDirection * Random.Range(settings.minVelocity, settings.maxVelocity);
            data.rotationSpeed = Random.Range(settings.minRotationSpeed, settings.maxRotationSpeed);
            if (Random.value < 0.5f) data.rotationSpeed *= -1;
            data.lifetime = Random.Range(settings.minLifetime, settings.maxLifetime);
            data.startScale = startScale;
            data.endScale = Random.Range(settings.minEndScale, settings.maxEndScale);

            return particle;
        }

        private string[] GetObstacleParticlePaths(string obstacleType)
        {
            switch (obstacleType.ToLower())
            {
                case "box":
                    return new string[]
                    {
                        "Obstacles/Box/Particles/particle_box_01",
                        "Obstacles/Box/Particles/particle_box_02",
                        "Obstacles/Box/Particles/particle_box_03"
                    };
                case "stone":
                    return new string[]
                    {
                        "Obstacles/Stone/Particles/particle_stone_01",
                        "Obstacles/Stone/Particles/particle_stone_02",
                        "Obstacles/Stone/Particles/particle_stone_03"
                    };
                case "vase":
                    return new string[]
                    {
                        "Obstacles/Vase/Particles/particle_vase_01",
                        "Obstacles/Vase/Particles/particle_vase_02",
                        "Obstacles/Vase/Particles/particle_vase_03"
                    };
                default:
                    // Fallback to box particles if unknown type
                    return new string[]
                    {
                        "Obstacles/Box/Particles/particle_box_01",
                        "Obstacles/Box/Particles/particle_box_02",
                        "Obstacles/Box/Particles/particle_box_03"
                    };
            }
        }

        private GameObject CreateSingleParticle(Vector3 center, Sprite sprite, Color color, ParticleSettings settings)
        {
            GameObject particle = new GameObject("Particle");
            particle.transform.position = center;

            SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            sr.sortingOrder = 10;

            // Random initial scale
            float startScale = Random.Range(settings.minStartScale, settings.maxStartScale);
            particle.transform.localScale = Vector3.one * startScale;

            // Store initial values in a component
            ParticleData data = particle.AddComponent<ParticleData>();
            data.velocity = Random.insideUnitCircle.normalized * Random.Range(settings.minVelocity, settings.maxVelocity);
            data.rotationSpeed = Random.Range(settings.minRotationSpeed, settings.maxRotationSpeed);
            if (Random.value < 0.5f) data.rotationSpeed *= -1; // Random direction
            data.lifetime = Random.Range(settings.minLifetime, settings.maxLifetime);
            data.startScale = startScale;
            data.endScale = Random.Range(settings.minEndScale, settings.maxEndScale);

            return particle;
        }

        private IEnumerator AnimateParticle(GameObject particle, ParticleSettings settings)
        {
            if (particle is null) yield break;

            ParticleData data = particle.GetComponent<ParticleData>();
            SpriteRenderer sr = particle.GetComponent<SpriteRenderer>();
            
            if (data is null || sr is null) yield break;

            float elapsedTime = 0f;
            Vector3 velocity = new Vector3(data.velocity.x, data.velocity.y, 0);
            Color originalColor = sr.color;

            while (elapsedTime < data.lifetime && particle != null)
            {
                float t = elapsedTime / data.lifetime;

                // Update position with gravity
                velocity.y += settings.gravity * Time.deltaTime;
                particle.transform.position += velocity * Time.deltaTime;

                // Update rotation
                particle.transform.Rotate(0, 0, data.rotationSpeed * Time.deltaTime);

                // Update scale using curve
                float scaleProgress = settings.scaleCurve.Evaluate(t);
                float currentScale = Mathf.Lerp(data.startScale, data.endScale, scaleProgress);
                particle.transform.localScale = Vector3.one * currentScale;

                // Update alpha using curve
                float alphaProgress = settings.alphaCurve.Evaluate(t);
                Color currentColor = originalColor;
                currentColor.a = alphaProgress;
                sr.color = currentColor;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (particle is not null)
            {
                Destroy(particle);
            }
        }

        private void CreateBurstEffect(Vector3 position, Color color, float intensity = 1f)
        {
            GameObject burst = new GameObject("BurstEffect");
            burst.transform.position = position;

            SpriteRenderer sr = burst.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite();
            sr.color = color;
            sr.sortingOrder = 15;

            StartCoroutine(AnimateBurst(burst, intensity));
        }

        public Sprite CreateCircleSprite()
        {
            int size = 64;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Bilinear;

            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f - 2f;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Vector2 pos = new Vector2(x, y);
                    float distance = Vector2.Distance(pos, center);
                    
                    if (distance <= radius)
                    {
                        float alpha = 1f - (distance / radius);
                        alpha = Mathf.Pow(alpha, 2);
                        texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        }

        private Sprite CreateLaserSprite(RocketDirection direction)
        {
            // Load the specific rocket laser beam sprite provided by the user
            Sprite laserSprite = Resources.Load<Sprite>("rocket_laser_beam");
            
            if (laserSprite is not null)
            {
                Debug.Log("Successfully loaded rocket_laser_beam.png sprite");
                return laserSprite;
            }
            
            Debug.LogWarning("rocket_laser_beam.png not found in Resources folder. Using fallback laser sprite.");
            
            int width = direction == RocketDirection.Horizontal ? 256 : 64;
            int height = direction == RocketDirection.Horizontal ? 64 : 256;
            
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Bilinear;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float alpha = 1f;
                    
                    // Create a realistic laser beam with bright core
                    if (direction == RocketDirection.Horizontal)
                    {
                        float distFromCenter = Mathf.Abs(y - height / 2f) / (height / 2f);
                        if (distFromCenter < 0.2f)
                        {
                            alpha = 1f; // Bright core
                        }
                        else if (distFromCenter < 0.6f)
                        {
                            alpha = Mathf.Lerp(1f, 0.4f, (distFromCenter - 0.2f) / 0.4f); // Mid glow
                        }
                        else
                        {
                            alpha = Mathf.Lerp(0.4f, 0f, (distFromCenter - 0.6f) / 0.4f); // Outer glow
                        }
                    }
                    else
                    {
                        float distFromCenter = Mathf.Abs(x - width / 2f) / (width / 2f);
                        if (distFromCenter < 0.2f)
                        {
                            alpha = 1f; // Bright core
                        }
                        else if (distFromCenter < 0.6f)
                        {
                            alpha = Mathf.Lerp(1f, 0.4f, (distFromCenter - 0.2f) / 0.4f); // Mid glow
                        }
                        else
                        {
                            alpha = Mathf.Lerp(0.4f, 0f, (distFromCenter - 0.6f) / 0.4f); // Outer glow
                        }
                    }
                    
                    alpha = Mathf.Clamp01(alpha);
                    texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        }

        private IEnumerator SpawnRocketTrailEffect(Vector3 position, Vector3 direction)
        {
            // Create enhanced trail particles coming from rocket backs
            string[] rocketParticles = { "Rocket/Particles/particle_smoke", "Rocket/Particles/particle_star" };
            
            int particleCount = Random.Range(3, 6);
            
            for (int i = 0; i < particleCount; i++)
            {
                string particlePath = rocketParticles[Random.Range(0, rocketParticles.Length)];
                Sprite particleSprite = Resources.Load<Sprite>(particlePath);
                
                if (particleSprite is not null)
                {
                    GameObject particle = new GameObject("TrailParticle");
                    Vector2 randomOffset = Random.insideUnitCircle * 0.15f;
                    particle.transform.position = position + new Vector3(randomOffset.x, randomOffset.y, 0f);

                    SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
                    sr.sprite = particleSprite;
                    
                    // Vary trail colors for more dynamic effect
                    Color trailColor = Random.value < 0.7f ? 
                        new Color(1f, 0.8f, 0.2f, 0.9f) : 
                        new Color(1f, 0.5f, 0.1f, 0.8f);
                    sr.color = trailColor;
                    sr.sortingOrder = 7;

                    float scale = Random.Range(0.1f, 0.2f); 
                    particle.transform.localScale = Vector3.one * scale;

                    StartCoroutine(AnimateTrailParticle(particle, direction));
                }
            }
            
            yield return null;
        }

        private IEnumerator AnimateTrailParticle(GameObject particle, Vector3 direction)
        {
            if (particle is null) yield break;

            SpriteRenderer sr = particle.GetComponent<SpriteRenderer>();
            
            // Enhanced velocity with some backwards motion for realistic trail
            Vector3 baseVelocity = direction * Random.Range(0.5f, 1.5f);
            Vector3 randomVelocity = (Vector3)Random.insideUnitCircle * 0.8f;
            Vector3 velocity = baseVelocity + randomVelocity;
            
            float lifetime = Random.Range(0.4f, 0.8f);
            float elapsed = 0f;
            Color originalColor = sr.color;
            float originalScale = particle.transform.localScale.x;

            // Add slight rotation
            float rotationSpeed = Random.Range(-180f, 180f);

            while (elapsed < lifetime && particle is not null)
            {
                float t = elapsed / lifetime;
                
                // Move particle with deceleration
                float currentSpeed = Mathf.Lerp(1f, 0.3f, t); // Slow down over time
                particle.transform.position += velocity * currentSpeed * Time.deltaTime;
                
                // Apply gravity effect
                velocity.y -= 2f * Time.deltaTime;
                
                // Rotate particle
                particle.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
                
                // Fade out with smooth curve
                Color color = originalColor;
                color.a = Mathf.Lerp(originalColor.a, 0f, Mathf.Pow(t, 0.5f));
                sr.color = color;
                
                // Shrink gradually
                float scale = Mathf.Lerp(originalScale, 0.02f, Mathf.Pow(t, 1.5f));
                particle.transform.localScale = Vector3.one * scale;

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (particle is not null)
            {
                Destroy(particle);
            }
        }

        private IEnumerator AnimateBurst(GameObject burst, float intensity)
        {
            if (burst is null) yield break;

            SpriteRenderer sr = burst.GetComponent<SpriteRenderer>();
            if (sr is null) yield break;

            float duration = 0.3f;
            float maxScale = 2f * intensity;
            Color originalColor = sr.color;

            float elapsed = 0f;
            while (elapsed < duration && burst != null)
            {
                float t = elapsed / duration;
                
                // Scale up quickly then fade
                float scale = Mathf.Lerp(0.1f, maxScale, t);
                burst.transform.localScale = Vector3.one * scale;

                // Fade out
                Color currentColor = originalColor;
                currentColor.a = Mathf.Lerp(0.8f, 0f, t * t);
                sr.color = currentColor;

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (burst != null)
            {
                Destroy(burst);
            }
        }

        /*
         * Helper component to store particle data
         */
        private class ParticleData : MonoBehaviour
        {
            public Vector2 velocity;
            public float rotationSpeed;
            public float lifetime;
            public float startScale;
            public float endScale;
        }
    }
} 