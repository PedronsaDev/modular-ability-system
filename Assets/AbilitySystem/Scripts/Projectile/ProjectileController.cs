using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _maxLifetime = 10f; // auto-destroy safety

    private Transform _target;
    private Rigidbody _rb;

    private AbilityData _ability;
    private GameObject _caster;

    public void Initialize(AbilityData ability, float speed, GameObject caster)
    {
        this._ability = ability;
        this._speed = speed;
        this._caster = caster;

        _rb = GetComponent<Rigidbody>();
        if (_rb)
        {
            _rb.useGravity = false;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;

            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        else
        {
            Debug.LogWarning("Projectile requires a Rigidbody component to detect collisions reliably.");
        }

        if (!TryGetComponent<Collider>(out _))
            Debug.LogWarning("Projectile requires a Collider component to detect collisions.");

        Destroy(gameObject, _maxLifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            return;

        if (other.gameObject.TryGetComponent<IDamageable>(out var target))
            _ability.Execute(_caster, target);

        Destroy(gameObject);
    }

    /// <summary>Maintain constant forward velocity each physics step.</summary>
    private void FixedUpdate()
    {
        _rb.linearVelocity = transform.forward*_speed;
    }
}
