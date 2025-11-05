using System;
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
        if (_currentStrategy is { IsTargeting: true })
            _currentStrategy.Update();
    }

    public void SetCurrentStrategy(TargetingStrategy strategy) => _currentStrategy = strategy;
    public void ClearCurrentStrategy() => _currentStrategy = null;
}