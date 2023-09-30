namespace CMIYC.Projectiles
{
    public interface IProjectileTarget
    {
        /// <summary>
        /// Called when a live projectile comes into contact with this GameObject.
        /// </summary>
        /// <param name="hitEvent">Projectile hit information.</param>
        void OnProjectileHit(ProjectileHitEvent hitEvent);
    }
}
