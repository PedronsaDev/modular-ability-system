using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityExecutor : MonoBehaviour
{
    [SerializeField] private AbilityData _ability;
    [SerializeField] private Enemy _target;
    [SerializeField] private AnimationController _animationController;

    private CountdownTimer _castTimer;

    private void Awake()
    {
        _animationController = GetComponent<AnimationController>();

        _castTimer = new CountdownTimer(_ability.CastTime);

        _castTimer.OnTimerStart = () => _animationController.PlayOneShot(_ability.CastAnimation);
        _castTimer.OnTimerStop = () => Cast(_ability, _target);
    }

    private void Cast(AbilityData ability, IDamageable target)
    {
        var targetMb = target as MonoBehaviour;

        if (ability.VFXProjectile && targetMb)
        {
            var projectileVFX = Instantiate(_ability.VFXProjectile, transform.position, transform.rotation);

            projectileVFX.SetTriggerCallback(co =>
            {
                _ability.Execute(this.gameObject, _target);
                if (ability.VFXOvertime)
                {
                    var overtimeVFX = Instantiate(_ability.VFXOvertime, targetMb.transform.position, Quaternion.identity, targetMb.transform);
                    Destroy(overtimeVFX, 3f);
                }
            });
        }
        else if (ability.VFXOvertime && targetMb)
        {
            _ability.Execute(this.gameObject, _target);
            if (ability.VFXOvertime)
            {
                var overtimeVFX = Instantiate(_ability.VFXProjectile, targetMb.transform.position, Quaternion.identity, targetMb.transform);
                Destroy(overtimeVFX, 3f);
            }
        }
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
