using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Simple beam link effect that creates a straight line between targets.
/// Good for laser or energy beam abilities.
/// </summary>
[Serializable]
public class BeamLinkEffect : IChainLinkEffect
{
    [Header("Beam Settings")]
    [Tooltip("Duration the beam lasts.")]
    public float Duration = 0.2f;

    [Tooltip("Width of the beam.")]
    public float Width = 0.08f;

    [Tooltip("Color of the beam.")]
    public Color BeamColor = Color.red;

    [Tooltip("Whether the beam should fade out over time.")]
    public bool FadeOut = true;

    public void CreateEffect(Vector3 startPos, Vector3 endPos, Transform startTransform = null, Transform endTransform = null)
    {
        GameObject beamObj = new GameObject("BeamLink");
        var lineRenderer = beamObj.AddComponent<LineRenderer>();

        // Configure line renderer
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // Set visual properties
        lineRenderer.startWidth = Width;
        lineRenderer.endWidth = Width;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Set color
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(BeamColor, 0f), new GradientColorKey(BeamColor, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );
        lineRenderer.colorGradient = gradient;

        // Handle fading and destruction
        if (FadeOut)
        {
            var fader = beamObj.AddComponent<BeamFader>();
            fader.Initialize(lineRenderer, BeamColor, Duration);
        }
        else
        {
            UnityEngine.Object.Destroy(beamObj, Duration);
        }
    }

    // Helper component for fading
    private class BeamFader : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private Color _startColor;
        private float _duration;
        private float _elapsed;

        public void Initialize(LineRenderer lr, Color color, float duration)
        {
            _lineRenderer = lr;
            _startColor = color;
            _duration = duration;
            _elapsed = 0f;
            StartCoroutine(FadeRoutine());
        }

        private IEnumerator FadeRoutine()
        {
            while (_elapsed < _duration)
            {
                _elapsed += Time.deltaTime;
                float alpha = 1f - (_elapsed / _duration);

                var gradient = new Gradient();
                var fadedColor = new Color(_startColor.r, _startColor.g, _startColor.b, alpha);
                gradient.SetKeys(
                    new[] { new GradientColorKey(fadedColor, 0f), new GradientColorKey(fadedColor, 1f) },
                    new[] { new GradientAlphaKey(alpha, 0f), new GradientAlphaKey(alpha, 1f) }
                );
                _lineRenderer.colorGradient = gradient;

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}

