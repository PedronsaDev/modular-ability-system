using System;
using UnityEngine;

/// <summary>
/// Particle trail link effect that spawns a particle system between targets.
/// Good for magical or elemental chain abilities.
/// </summary>
[Serializable]
public class ParticleTrailLinkEffect : IChainLinkEffect
{
    [Header("Particle Settings")]
    [Tooltip("Particle prefab to spawn. Should move from start to end position.")]
    public GameObject ParticlePrefab;

    [Tooltip("Duration before destroying the particle effect.")]
    public float Duration = 1f;

    [Tooltip("Speed at which particles travel (if using animated movement).")]
    public float TravelSpeed = 10f;

    [Tooltip("Spawn the particle at start and move it to end, or spawn at end facing start.")]
    public bool AnimateMovement = true;

    public void CreateEffect(Vector3 startPos, Vector3 endPos, Transform startTransform = null, Transform endTransform = null)
    {
        if (!ParticlePrefab)
        {
            Debug.LogWarning("ParticleTrailLinkEffect: No particle prefab assigned!");
            return;
        }

        Vector3 direction = (endPos - startPos).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        GameObject particleObj = UnityEngine.Object.Instantiate(ParticlePrefab, startPos, rotation);

        if (AnimateMovement)
        {
            var mover = particleObj.AddComponent<ParticleMover>();
            mover.Initialize(startPos, endPos, TravelSpeed, Duration);
        }
        else
        {
            UnityEngine.Object.Destroy(particleObj, Duration);
        }
    }

    // Helper component for moving particles
    private class ParticleMover : MonoBehaviour
    {
        private Vector3 _start;
        private Vector3 _end;
        private float _speed;
        private float _duration;
        private float _elapsed;

        public void Initialize(Vector3 start, Vector3 end, float speed, float duration)
        {
            _start = start;
            _end = end;
            _speed = speed;
            _duration = duration;
            _elapsed = 0f;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;

            if (_elapsed >= _duration)
            {
                Destroy(gameObject);
                return;
            }

            // Move towards target
            float t = _elapsed * _speed / Vector3.Distance(_start, _end);
            t = Mathf.Clamp01(t);
            transform.position = Vector3.Lerp(_start, _end, t);

            // Update rotation to face movement direction
            if (t < 1f)
            {
                Vector3 direction = (_end - transform.position).normalized;
                if (direction.sqrMagnitude > 0.01f)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }
            }
        }
    }
}

