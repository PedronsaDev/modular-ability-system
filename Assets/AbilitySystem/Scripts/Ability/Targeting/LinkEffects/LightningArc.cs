using System.Collections;
using UnityEngine;

/// <summary>
/// Creates an animated lightning arc effect between two points using LineRenderer.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LightningArc : MonoBehaviour
{
    [Header("Arc Settings")]
    [Tooltip("Start position of the arc.")]
    public Vector3 StartPosition;

    [Tooltip("End position of the arc.")]
    public Vector3 EndPosition;

    [Tooltip("How many segments the arc should have (more = smoother but more expensive).")]
    public int Segments = 20;

    [Tooltip("Maximum displacement of arc segments from the straight line.")]
    public float Intensity = 0.5f;

    [Tooltip("How fast the arc animates (lower = more chaotic).")]
    public float AnimationSpeed = 0.05f;

    [Tooltip("Duration before the arc destroys itself.")]
    public float Duration = 0.3f;

    [Header("Visual")]
    [Tooltip("Width curve for the line (start to end).")]
    public AnimationCurve WidthCurve = AnimationCurve.Constant(0, 1, 0.1f);

    [Tooltip("Color gradient for the arc.")]
    public Gradient ColorGradient;

    private LineRenderer _lineRenderer;
    private float _lifetime;
    private Coroutine _animationCoroutine;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }

    private void Start()
    {
        if (_animationCoroutine == null)
            _animationCoroutine = StartCoroutine(AnimateArc());
    }

    private void SetupLineRenderer()
    {
        if (!_lineRenderer)
            return;

        _lineRenderer.positionCount = Segments + 1;
        _lineRenderer.useWorldSpace = true;

        // Apply width curve
        _lineRenderer.widthCurve = WidthCurve;

        // Apply color gradient if set
        if (ColorGradient != null && ColorGradient.colorKeys.Length > 0)
        {
            _lineRenderer.colorGradient = ColorGradient;
        }
        else
        {
            // Default cyan/white gradient
            var gradient = new Gradient();
            gradient.SetKeys(
                new[] { new GradientColorKey(Color.cyan, 0f), new GradientColorKey(Color.white, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
            );
            _lineRenderer.colorGradient = gradient;
        }
    }

    /// <summary>
    /// Initializes and starts the lightning arc effect.
    /// </summary>
    public void Initialize(Vector3 start, Vector3 end, float duration = 0.3f, float intensity = 0.5f)
    {
        StartPosition = start;
        EndPosition = end;
        Duration = duration;
        Intensity = intensity;
        _lifetime = 0f;

        if (_lineRenderer)
        {
            SetupLineRenderer();
        }

        if (_animationCoroutine == null && gameObject.activeInHierarchy)
        {
            _animationCoroutine = StartCoroutine(AnimateArc());
        }
    }

    private IEnumerator AnimateArc()
    {
        while (_lifetime < Duration)
        {
            GenerateLightningPath();
            _lifetime += AnimationSpeed;
            yield return new WaitForSeconds(AnimationSpeed);
        }

        Destroy(gameObject);
    }

    private void GenerateLightningPath()
    {
        if (!_lineRenderer)
            return;

        Vector3[] positions = new Vector3[Segments + 1];
        Vector3 direction = EndPosition - StartPosition;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        // If the direction is too vertical, use forward instead
        if (perpendicular.sqrMagnitude < 0.01f)
            perpendicular = Vector3.Cross(direction, Vector3.forward).normalized;

        for (int i = 0; i <= Segments; i++)
        {
            float t = i / (float)Segments;
            Vector3 basePosition = Vector3.Lerp(StartPosition, EndPosition, t);

            // Add random offset perpendicular to the line
            // Less offset at the start and end
            float offsetStrength = Mathf.Sin(t * Mathf.PI) * Intensity;
            Vector3 randomOffset = perpendicular * Random.Range(-offsetStrength, offsetStrength);

            // Add some 3D variation
            Vector3 up = Vector3.up * Random.Range(-offsetStrength * 0.5f, offsetStrength * 0.5f);

            positions[i] = basePosition + randomOffset + up;
        }

        // Ensure start and end are exact
        positions[0] = StartPosition;
        positions[Segments] = EndPosition;

        _lineRenderer.SetPositions(positions);
    }

    /// <summary>
    /// Creates a lightning arc between two transforms.
    /// </summary>
    public static LightningArc Create(Transform start, Transform end, float duration = 0.3f, float intensity = 0.5f, Vector3 startOffset = default, Vector3 endOffset = default)
    {
        GameObject arcObj = new GameObject("LightningArc");
        var arc = arcObj.AddComponent<LightningArc>();

        // Add LineRenderer and configure it
        var lr = arcObj.GetComponent<LineRenderer>();
        if (!lr)
            lr = arcObj.AddComponent<LineRenderer>();

        // Set material to default or use a better one if available
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = 0.1f;
        lr.endWidth = 0.05f;

        arc.Initialize(start.position + startOffset, end.position + endOffset, duration, intensity);
        return arc;
    }

    /// <summary>
    /// Creates a lightning arc between two positions.
    /// </summary>
    public static LightningArc Create(Vector3 start, Vector3 end, float duration = 0.3f, float intensity = 0.5f)
    {
        GameObject arcObj = new GameObject("LightningArc");
        var arc = arcObj.AddComponent<LightningArc>();

        // Add LineRenderer and configure it
        var lr = arcObj.GetComponent<LineRenderer>();
        if (!lr)
            lr = arcObj.AddComponent<LineRenderer>();

        // Set material to default or use a better one if available
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = 0.1f;
        lr.endWidth = 0.05f;

        arc.Initialize(start, end, duration, intensity);
        return arc;
    }
}

