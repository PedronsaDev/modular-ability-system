# Architecture Overview
- Editor visualization of targeting radius & chain links.
- ScriptableObject-based balance sheets for ability parameters.
- Burst / Jobs for large-scale targeting queries.
- Object pooling for projectiles & VFX.
## Future Improvements

- IntervalTimer batches interval checks using a while loop to handle low frame rates.
- ChainTargeting uses a reusable collider buffer to reduce GC pressure.
- Timers avoid per-MonoBehaviour overhead by central Update loop.
## Performance Notes

```mermaid
  ProjectileController --> AD
  TS -->|Execute| AD
  PAC --> UI[AbilityUI / SlotUI]
  RE --> TM[TimerManager]
  EF --> RE[IEffect Runtime]
  AD --> EF[IEffectFactory]
  AD --> TS[TargetingStrategy]
  AS --> AD[AbilityData]
  PAC[PlayerAbilityCaster] --> AS[AbilitySlot]
graph TD
```
Place into README when exported as PNG/SVG:
## Mermaid Diagram Stub

```
ProjectileController --> AbilityData.Execute --> Effects
AbilityUI --> AbilitySlotUI --> AbilitySlot
TimerBootstrapper --> PlayerLoopSystem --> TimerManager.UpdateTimers
IEffect (HealEffectOvertime) --> IntervalTimer
AbilitySlot --> CountdownTimer
PlayerAbilityCaster --> AbilitySlot --> AbilityData
AbilityData --> IEffectFactory --> IEffect
AbilityData --> TargetingStrategy
```

A UML component diagram (or Mermaid) showing relationships:
## Suggested Diagram

TimerBootstrapper inserts TimerManager.UpdateTimers into the Update phase of Unity's PlayerLoop at runtime, allowing timers that are not MonoBehaviours to tick without attaching scripts to GameObjects.
## Player Loop Integration

- Add validation/editor tooling (custom inspectors already present for AbilityData & effects).
- Integrate new feedback (audio/VFX) via AbilityData.EffectVFX or strategy-specific prefabs.
- Create new Effect runtime + Factory for new gameplay behavior.
- Add new TargetingStrategy subclass (e.g., ConeTargeting) and assign in AbilityData asset.
## Extensibility Points

8. UI components react to AbilitySlot events to reflect cooldown & icon changes.
7. AbilitySlot subscribes to TargetingStrategy.OnTargetingCompleted to start cooldown.
6. Effects may start timers (e.g., IntervalTimer) and signal completion via OnCompleted.
5. AbilityData creates runtime effects via factories and applies them to IDamageable target.
4. TargetingStrategy selects targets and calls AbilityData.Execute(caster, target).
3. AbilityData.Target delegates to TargetingStrategy.Start.
2. Cast time handled by CountdownTimer; on completion StartTargeting invoked.
1. Input triggers PlayerAbilityCaster.TryExecute(slot).
## Data Flow

- ProjectileController: Handles projectile movement & impact; delegates ability execution to hit target.
- Damageables: PlayerHealth / Enemy implement IDamageable to interop with effect application and health modification.
- UI Layer: AbilityUI (inventory + binding slots), AbilitySlotUI (individual slot visualization with cooldown tween), DraggableAbilityUI (drag & drop assignment), DamagePopup (feedback).
- PlayerAbilityCaster: Orchestrates input → cast timer → targeting start → ability execution.
- AbilitySlot: Holds an equipped AbilityData; manages cooldown via CountdownTimer and exposes CanUse state & events.
- Timers: Custom lightweight timer framework (Timer, CountdownTimer, IntervalTimer) integrated into Unity's PlayerLoop via TimerBootstrapper → TimerManager.UpdateTimers.
- Effects & Factories: Runtime effect instances produced by factories (e.g., KnockbackEffectFactory, HealEffectOvertimeFactory). Each effect implements IEffect<TTarget> allowing application & cancellation lifecycle.
- TargetingStrategy (polymorphic): Strategy pattern implementing how targets are selected / how ability executes (Self, MouseAOE, Projectile, Chain, etc.).
- AbilityData (ScriptableObject): Declarative definition of an ability (icon, description, cast time, cooldown, effect factories, targeting strategy).
## Core Concepts

This document describes the high-level architecture of the Modular Ability System.


