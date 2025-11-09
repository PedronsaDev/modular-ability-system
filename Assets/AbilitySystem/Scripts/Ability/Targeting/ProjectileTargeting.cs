using UnityEngine;

public class ProjectileTargeting : TargetingStrategy
{
    public GameObject ProjectilePrefab;
    public float ProjectileSpeed = 20f;

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

    public override void Cancel()
    {
        _isTargeting = false;

        RaiseTargetingComplete();
        TargetingManager.ClearCurrentStrategy();
    }
}
