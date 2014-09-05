using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{

    protected Transform trans;

    void Awake()
    {
        trans = transform;
    }

    public virtual IEnumerator MoveProjectile()
    {
        yield return null;
    }

}
