using UnityEngine;

/// <summary>
/// Spawns a projectile oriented to camera forward; projectile impact executes ability effects.
/// </summary>
public class ProjectileTargeting : TargetingStrategy
{
    public GameObject ProjectilePrefab;
    public float ProjectileSpeed = 20f;

    /// <summary>Starts projectile targeting by instantiating the projectile prefab.</summary>
    public override void Start(AbilityData ability, TargetingManager targetingManager, GameObject caster)
    {
        this.TargetingManager = targetingManager;
        this.Ability = ability;

        if (ProjectilePrefab)
        {
            var flatForward = targetingManager.Cam.transform.forward.normalized;
            flatForward.y = 0;
            var forwardRotation = Quaternion.LookRotation(flatForward);
            var projectile = Object.Instantiate(ProjectilePrefab, caster.transform.position, forwardRotation);

            projectile.GetComponent<ProjectileController>().Initialize(Ability, ProjectileSpeed ,caster);
        }

        RaiseTargetingComplete();
    }

    /// <summary>Cancels targeting (cooldown trigger) though projectile is already fired.</summary>
    public override void Cancel()
    {
        _isTargeting = false;

        RaiseTargetingComplete();
        TargetingManager.ClearCurrentStrategy();
    }
}
