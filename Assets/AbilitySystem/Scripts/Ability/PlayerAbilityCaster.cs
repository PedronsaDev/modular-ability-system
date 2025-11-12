using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using YourNamespace;

/// <summary>
/// Handles player input to cast abilities: resolves slot from input, runs cast timer, starts targeting.
/// </summary>
[RequireComponent(typeof(TargetingManager))]
public class PlayerAbilityCaster : MonoBehaviour
{
    [SerializeField] private AbilityData _debugAbility;

    private readonly AbilitySlot[] _abilitySlots = new AbilitySlot[10];
    private PlayerAnimationController _playerAnimationController;
    private TargetingManager _targetingManager;
    private CountdownTimer _castTimer;

    private PlayerInputActions _controls;

    private void Awake()
    {
        _playerAnimationController = GetComponent<PlayerAnimationController>();
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

        if (_debugAbility)
            EquipDebugAbility();
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

    /// <summary>Equip the debug ability into slot 0 (editor helper).</summary>
    [Button]
    public void EquipDebugAbility() => EquipAbility(_debugAbility, 0);

    /// <summary>Equip an ability into a specified slot index.</summary>
    /// <param name="ability">Ability asset to equip.</param>
    /// <param name="slot">Slot index (0-based).</param>
    public void EquipAbility(AbilityData ability, int slot)
    {
        if (slot < 0 || slot >= _abilitySlots.Length)
        {
            Debug.LogWarning("Invalid ability slot: " + slot);
            return;
        }

        _abilitySlots[slot].Initialize(ability);
    }

    /// <summary>Starts targeting for the provided slot's ability after cast animation finishes.</summary>
    private void StartTargeting(AbilitySlot slot)
    {
        slot.Ability.Target(_targetingManager, this.gameObject);

        // play casting sfx and vfx if any
    }

    /// <summary>Attempt to execute an ability: validates slot usability then runs its cast timer.</summary>
    private void TryExecute(AbilitySlot slot)
    {
        if (slot == null || !slot.CanUse)
            return;

        if (_castTimer is { IsRunning: true })
        {
            _castTimer.Pause();
        }

        _castTimer = new CountdownTimer(slot.Ability.CastTime);
        _castTimer.OnTimerStart = () => _playerAnimationController.PlayOneShot(slot.Ability.CastAnimation);
        _castTimer.OnTimerStop = () => StartTargeting(slot);

        _castTimer.Start();
    }
}