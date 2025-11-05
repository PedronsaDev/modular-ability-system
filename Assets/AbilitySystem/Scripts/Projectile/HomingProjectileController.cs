using UnityEngine;

public class HomingProjectileController : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _maxLifetime = 10f; // auto-destroy safety

    private Transform _target;
    private Rigidbody _rb;

    private AbilityData _ability;
    private GameObject _caster;

    public void Initialize(AbilityData ability, float speed, GameObject caster, Transform target)
    {
        this._ability = ability;
        this._speed = speed;
        this._caster = caster;
        this._target = target;

        _rb = GetComponent<Rigidbody>();
        if (_rb)
        {
            _rb.useGravity = false;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;

            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        else
            Debug.LogWarning("Projectile requires a Rigidbody component to detect collisions reliably.");

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

    private void FixedUpdate()
    {
        if (_target)
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 5f);
        }

        _rb.linearVelocity = transform.forward * _speed;
    }
}
