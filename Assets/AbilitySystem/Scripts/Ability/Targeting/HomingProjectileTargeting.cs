using UnityEngine;

public class HomingProjectileTargeting : TargetingStrategy
{
    public GameObject HomingProjectilePrefab;
    public float ProjectileSpeed = 20f;

    public override void Start(AbilityData ability, TargetingManager targetingManager)
    {
        this.TargetingManager = targetingManager;
        this.Ability = ability;

        GameObject caster = this.TargetingManager.gameObject;

        if (HomingProjectilePrefab)
        {
            var flatForward = targetingManager.Cam.transform.forward.normalized;
            flatForward.y = 0;
            var forwardRotation = Quaternion.LookRotation(flatForward);
            var projectile = Object.Instantiate(HomingProjectilePrefab, caster.transform.position + Vector3.up, forwardRotation);

            projectile.GetComponent<ProjectileController>().Initialize(Ability, ProjectileSpeed, caster);
        }
    }

}
