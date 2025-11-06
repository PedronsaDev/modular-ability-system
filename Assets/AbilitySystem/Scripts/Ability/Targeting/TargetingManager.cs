using UnityEngine;
using UnityEngine.InputSystem;

public class TargetingManager : MonoBehaviour
{
    public InputActionAsset Input;
    public Camera Cam;

    private TargetingStrategy _currentStrategy;

    public InputAction ClickAction => Input.FindAction("Click");

    private void Awake()
    {
        if (!Cam)
            Cam = Camera.main;

        ClickAction.Enable();
    }

    private void Update()
    {
        if (_currentStrategy != null && _currentStrategy.IsTargeting)
            _currentStrategy.Update();
    }

    public void SetCurrentStrategy(TargetingStrategy strategy)
    {
        Debug.Log("Setting current targeting strategy");
        _currentStrategy?.Cancel();
        _currentStrategy = strategy;
    }
    public void ClearCurrentStrategy() => _currentStrategy = null;
}