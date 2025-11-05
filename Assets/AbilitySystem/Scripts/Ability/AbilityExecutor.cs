using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TargetingManager))]
public class AbilityExecutor : MonoBehaviour
{
    [SerializeField] private AbilityData _ability;
    [SerializeField] private Enemy _target;
    [SerializeField] private AnimationController _animationController;

    private TargetingManager _targetingManager;
    private CountdownTimer _castTimer;

    private void Awake()
    {
        _animationController = GetComponent<AnimationController>();
        _targetingManager = GetComponent<TargetingManager>();

        _castTimer = new CountdownTimer(_ability.CastTime);

        _castTimer.OnTimerStart = () => _animationController.PlayOneShot(_ability.CastAnimation);
        _castTimer.OnTimerStop = () => Cast(_ability);
    }

    private void Cast(AbilityData ability)
    {
        // start the targeting process
        ability.Target(_targetingManager);

        // play cast animation in loop
        // play casting sfx and vfx if any
    }



    public void Execute()
    {
        Debug.Log("Executing ability: " + _ability.Label);
        _castTimer.Start();
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            Execute();
    }
}
