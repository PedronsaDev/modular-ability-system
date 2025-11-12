using UnityEngine;

/// <summary>
/// Example: Chain effect that combines multiple visual effects.
/// Shows how to create advanced custom effects by combining existing ones.
/// Create instances via: Assets > Create > Chain Link Effects > Combo Effect
/// </summary>
[CreateAssetMenu(fileName = "New Combo Effect", menuName = "Chain Link Effects/Combo Effect (Example)", order = 100)]
public class ComboLinkEffect : ChainLinkEffectBase
{
    [Header("Primary Effect")]
    [Tooltip("Main visual effect (e.g., lightning arc).")]
    public LightningArcLinkEffect PrimaryEffect = new LightningArcLinkEffect();

    [Header("Secondary Effect")]
    [Tooltip("Additional overlay effect (e.g., beam).")]
    public BeamLinkEffect SecondaryEffect = new BeamLinkEffect();

    [Header("Settings")]
    [Tooltip("Enable the secondary effect.")]
    public bool UseSecondaryEffect = true;

    [Tooltip("Delay before spawning secondary effect (seconds).")]
    public float SecondaryDelay = 0.05f;

    public override void CreateEffect(Vector3 startPos, Vector3 endPos, Transform startTransform = null, Transform endTransform = null)
    {
        // Always create primary effect
        PrimaryEffect?.CreateEffect(startPos, endPos, startTransform, endTransform);

        // Optionally create secondary effect with delay
        if (UseSecondaryEffect && SecondaryEffect != null)
        {
            if (SecondaryDelay > 0f)
            {
                // Create a helper GameObject to delay the secondary effect
                var delayHelper = new GameObject("ComboEffectDelayHelper");
                var delayer = delayHelper.AddComponent<DelayedEffectSpawner>();
                delayer.Initialize(SecondaryEffect, startPos, endPos, startTransform, endTransform, SecondaryDelay);
            }
            else
            {
                SecondaryEffect.CreateEffect(startPos, endPos, startTransform, endTransform);
            }
        }
    }

    /// <summary>
    /// Helper component to spawn an effect after a delay.
    /// </summary>
    private class DelayedEffectSpawner : MonoBehaviour
    {
        private IChainLinkEffect _effect;
        private Vector3 _start;
        private Vector3 _end;
        private Transform _startTransform;
        private Transform _endTransform;
        private float _delay;
        private float _elapsed;

        public void Initialize(
            IChainLinkEffect effect, Vector3 start, Vector3 end,
            Transform startTransform, Transform endTransform, float delay)
        {
            _effect = effect;
            _start = start;
            _end = end;
            _startTransform = startTransform;
            _endTransform = endTransform;
            _delay = delay;
            _elapsed = 0f;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;

            if (_elapsed >= _delay)
            {
                // Update positions if transforms still exist and have moved
                Vector3 finalStart = _startTransform ? _startTransform.position : _start;
                Vector3 finalEnd = _endTransform ? _endTransform.position : _end;

                _effect?.CreateEffect(finalStart, finalEnd, _startTransform, _endTransform);
                Destroy(gameObject);
            }
        }
    }
}
