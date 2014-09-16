using UnityEngine;
using System.Collections;
using System;

public class Projectile_Laser : Projectile
{
    //Transform trans;

    //void Awake()
    //{
    //    trans = transform;
    //}
    public IEnumerator MoveProjectile(Vector3 destination, float period,Action OnComplete )
    {
        float time = 0f;
        Vector3 startPos = trans.position;
        while(time<1f)
        {
            trans.position = Vector3.Lerp(startPos, destination, time);
            time += Time.deltaTime / period;
            yield return null;
        }
        if(OnComplete!=null)
        {
            OnComplete();
        }
        Destroy(gameObject);
    }

    //void OnTriggerEnter(Collider col)
    //{
    //    if (col.tag == GlobalTagsAndLayers.Instance.tags.enemyShipTag)
    //    {
    //        OnProjectileHit();

    //    }
    //}


}
