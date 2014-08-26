using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{


    public virtual IEnumerator MoveProjectile()
    {
        yield return null;
    }

}
