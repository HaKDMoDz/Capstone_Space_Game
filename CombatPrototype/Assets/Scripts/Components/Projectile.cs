using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{

    public delegate void ProjectileHit();
    public event ProjectileHit ProjectileHitEvent = new ProjectileHit(delegate() { });

    protected virtual void OnProjectileHit()
    {
        ProjectileHit invoker = ProjectileHitEvent;
        invoker();
    }



    public virtual IEnumerator MoveProjectile()
    {
        yield return null;
    }

}
