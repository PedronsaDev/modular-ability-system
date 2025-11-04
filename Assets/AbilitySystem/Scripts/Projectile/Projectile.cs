using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private bool _isHoming; // default false; no need to initialize
    [SerializeField] private float _speed = 10f;
    [SerializeField] private Transform _target;
    [SerializeField] private float _maxLifetime = 10f; // auto-destroy safety

    private Action<Collision> _onCollisionCallback;
    private Action<Collider> _onTriggerCallback;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb)
        {
            _rb.useGravity = false;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;

            // Use continuous collision detection to reduce tunneling at high speed
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        else
        {
            Debug.LogWarning("Projectile requires a Rigidbody component to detect collisions reliably.");
        }

        // Ensure there's a collider
        if (!TryGetComponent<Collider>(out _))
            Debug.LogWarning("Projectile requires a Collider component to detect collisions.");
    }

    private void OnEnable()
    {
        if (_maxLifetime > 0f)
            Destroy(gameObject, _maxLifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            return;

        // Debug.Log("Projectile triggered with: " + other.gameObject.name);
        _onTriggerCallback?.Invoke(other);
        Destroy(gameObject);
    }

    public void SetTriggerCallback(Action<Collider> action, Transform target = null)
    {
        _onTriggerCallback = action;
        if (target) _target = target;
    }

    private void FixedUpdate()
    {
        if (!_rb)
        {
            // Fallback to transform-based movement (less reliable for collisions)
            if (_isHoming && _target)
            {
                Vector3 direction = (_target.position - transform.position).normalized;
                transform.position += direction*(_speed*Time.fixedDeltaTime);
                transform.rotation = Quaternion.LookRotation(direction);
            }
            else
            {
                transform.position += transform.forward*(_speed*Time.fixedDeltaTime);
            }
            return;
        }

        Vector3 moveDir;
        if (_isHoming && _target)
        {
            moveDir = (_target.position - transform.position).normalized;
            // Face the movement direction when homing
            if (moveDir.sqrMagnitude > 0.0001f)
                _rb.MoveRotation(Quaternion.LookRotation(moveDir));
        }
        else
        {
            moveDir = transform.forward;
        }

        _rb.linearVelocity = moveDir*_speed;
    }
}
