using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TargetingManager))]
public class PlayerAbilityCaster : MonoBehaviour
{
    [SerializeField] private AbilityData _debugAbility;
    [SerializeField] private Enemy _target;
    [SerializeField] private AnimationController _animationController;

    private readonly AbilitySlot[] _abilitySlots = new AbilitySlot[10];
    private TargetingManager _targetingManager;
    private CountdownTimer _castTimer;

    private PlayerInputActions _controls;

    private void Awake()
    {
        _animationController = GetComponent<AnimationController>();
        _targetingManager = GetComponent<TargetingManager>();

        _controls = new PlayerInputActions();
    }

    private void Start()
    {
        if (_abilitySlots.Length == 0)
            Debug.LogWarning("No abilities equipped.");

        _controls.Abilities.Enable();
        _controls.Abilities.UseAbility.performed += OnAbilityPerformed;

        for (int i = 0; i < _abilitySlots.Length; i++)
            _abilitySlots[i] = new AbilitySlot();

        FindAnyObjectByType<AbilityUI>().BindSlots(_abilitySlots);
    }

    private void OnDestroy()
    {
        _controls.Abilities.UseAbility.performed -= OnAbilityPerformed;
        _controls.Abilities.Disable();
    }

    private void OnAbilityPerformed(InputAction.CallbackContext ctx)
    {
        var control = ctx.control;
        string bindingName = control.name;

        int slotIndex = GetSlotFromBinding(bindingName);

        if (slotIndex >= 0 && slotIndex < _abilitySlots.Length)
            TryExecute(_abilitySlots[slotIndex]);
    }

    private int GetSlotFromBinding(string keyName)
    {
        return keyName switch
        {
            "1" => 0,
            "2" => 1,
            "3" => 2,
            "4" => 3,
            "5" => 4,
            "6" => 5,
            "7" => 6,
            "8" => 7,
            "9" => 8,
            "0" => 9,
            _ => -1
        };
    }

    [Button]
    public void EquipDebugAbility() => EquipAbility(_debugAbility, 0);


    public void EquipAbility(AbilityData ability, int slot)
    {
        if (slot < 0 || slot >= _abilitySlots.Length)
        {
            Debug.LogWarning("Invalid ability slot: " + slot);
            return;
        }

        _abilitySlots[slot].Initialize(ability);
    }

    private void Cast(AbilitySlot slot)
    {
        slot.StartCooldown();
        slot.Ability.Target(_targetingManager);

        // play cast animation in loop
        // play casting sfx and vfx if any
    }

    private void TryExecute(AbilitySlot slot)
    {
        if (slot == null || !slot.CanUse)
            return;

        _castTimer = new CountdownTimer(slot.Ability.CastTime);
        _castTimer.OnTimerStart = () => _animationController.PlayOneShot(slot.Ability.CastAnimation);
        _castTimer.OnTimerStop = () => Cast(slot);

        Debug.Log("Executing ability: " + slot.Ability.Label);
        _castTimer.Start();
    }
}