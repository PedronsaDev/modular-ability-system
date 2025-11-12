using System;

namespace YourNamespace
{
    /// <summary>
    /// Holds an equipped <see cref="AbilityData"/> and manages its cooldown state.
    /// Provides events for UI (initialize, cooldown start/complete).
    /// </summary>
    [Serializable]
    public class AbilitySlot
    {
        /// <summary>Gets the ability data associated with this slot.</summary>
        public AbilityData Ability { get; private set; }

        /// <summary>Indicates if the ability can be used (not on cooldown).</summary>
        public bool CanUse { get; private set; }

        private CountdownTimer _cooldownTimer;

        /// <summary>Triggered when the cooldown starts.</summary>
        public event Action OnCooldownStart;

        /// <summary>Triggered when the cooldown completes.</summary>
        public event Action OnCooldownComplete;

        /// <summary>Event called when the ability slot is initialized with an ability.</summary>
        public event Action<AbilitySlot> OnInitialize;

        /// <summary>
        /// Initializes the slot with a new ability and subscribes to targeting completion for cooldown.
        /// </summary>
        /// <param name="ability">Ability to place in the slot.</param>
        public void Initialize(AbilityData ability)
        {
            if (Ability)
            {
                Ability.TargetingStrategy.OnTargetingCompleted -= StartCooldown;
            }

            Ability = ability;
            CanUse = true;
            Ability.TargetingStrategy.OnTargetingCompleted += StartCooldown;
            OnInitialize?.Invoke(this);
        }

        /// <summary>Begins cooldown if the ability has a positive cooldown time.</summary>
        private void StartCooldown()
        {
            if (Ability.CooldownTime <= 0)
                return;

            OnCooldownStart?.Invoke();

            CanUse = false;
            _cooldownTimer = new CountdownTimer(Ability.CooldownTime);
            _cooldownTimer.OnTimerStop = () =>
            {
                CanUse = true;
                OnCooldownComplete?.Invoke();
            };

            _cooldownTimer.Start();
        }

        /// <summary>Convenience method to set and initialize an ability.</summary>
        /// <param name="abilityData">The ability data to set.</param>
        public void SetAbility(AbilityData abilityData)
        {
            Initialize(abilityData);
        }
    }
}
