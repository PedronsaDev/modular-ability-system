using UnityEngine;

public enum MovementState
{
    Idle,
    Moving
}

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _lookSensitivity = 0.1f;
    [SerializeField] private Transform _cameraAnchor;
    [Header("Animation")]
    [SerializeField] private PlayerAnimationController _playerAnimationController;
    [SerializeField] private float _animBlendDamp = 0.1f;

    private CharacterController _characterController;
    private PlayerInputActions _inputActions;
    private Vector3 _velocity;
    private MovementState _currentMovementState = MovementState.Idle;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        if (!_playerAnimationController)
            _playerAnimationController = GetComponentInChildren<PlayerAnimationController>();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        Vector2 input = _inputActions.Player.Move.ReadValue<Vector2>();

        Vector3 camForward = _cameraAnchor ? _cameraAnchor.forward : transform.forward;
        Vector3 camRight = _cameraAnchor ? _cameraAnchor.right : transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward = camForward.sqrMagnitude > 0f ? camForward.normalized : Vector3.forward;
        camRight = camRight.sqrMagnitude > 0f ? camRight.normalized : Vector3.right;

        Vector3 desiredVelocity = (camRight * input.x + camForward * input.y) * _moveSpeed;
        desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, _moveSpeed);

        Vector3 currentHorizontal = new Vector3(_velocity.x, 0f, _velocity.z);
        Vector3 targetHorizontal = new Vector3(desiredVelocity.x, 0f, desiredVelocity.z);

        float groundAcceleration = 25f;
        Vector3 newHorizontal = Vector3.MoveTowards(currentHorizontal, targetHorizontal, groundAcceleration*Time.deltaTime);
        _velocity.x = newHorizontal.x;
        _velocity.z = newHorizontal.z;
        _velocity.y += -30*Time.deltaTime; // temporary gravity
        _characterController.Move(_velocity*Time.deltaTime);

        UpdateMovementStateAndAnimation(camForward, camRight);
    }

    private void UpdateMovementStateAndAnimation(Vector3 camForward, Vector3 camRight)
    {
        Vector3 horizontalVel = new Vector3(_velocity.x, 0f, _velocity.z);
        float speed = horizontalVel.magnitude;
        MovementState newState = speed > 0.05f ? MovementState.Moving : MovementState.Idle;

        if (newState != _currentMovementState)
        {
            _currentMovementState = newState;
            _playerAnimationController?.SetMovementState(_currentMovementState);
        }

        if (_playerAnimationController)
        {
            // Project velocity onto camera axes to get blend values in local camera space
            Vector2 blend = Vector2.zero;
            if (speed > 0.001f)
            {
                Vector3 dir = horizontalVel / Mathf.Max(speed, 0.0001f);
                float forwardAmt = Vector3.Dot(dir, camForward);
                float rightAmt = Vector3.Dot(dir, camRight);
                blend = new Vector2(rightAmt, forwardAmt) * Mathf.Clamp01(speed / _moveSpeed);
            }
            _playerAnimationController.SetMoveBlend(blend, _animBlendDamp);
        }
    }

    private void HandleRotation()
    {
        Vector2 input = _inputActions.Player.Look.ReadValue<Vector2>();

        transform.Rotate(Vector3.up*(input.x*_lookSensitivity));
        if (_cameraAnchor)
        {
            _cameraAnchor.Rotate(Vector3.right*(-input.y*_lookSensitivity));
            Vector3 currentRotation = _cameraAnchor.localEulerAngles;
            if (currentRotation.x > 180f)
                currentRotation.x -= 360f;
            currentRotation.x = Mathf.Clamp(currentRotation.x, -80f, 80f);
            _cameraAnchor.localEulerAngles = new Vector3(currentRotation.x, 0f, 0f);
        }
    }
}
