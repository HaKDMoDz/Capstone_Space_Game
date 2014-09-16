using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Projectile_Missile : Projectile 
{

    public IEnumerator MoveProjectile(Vector3 destination, float period, Action OnComplete)
    {
        float time = 0f;
        Vector3 startPos = trans.position;
        while (time < 1f)
        {
            trans.position = Vector3.Lerp(startPos, destination, time);
            time += Time.deltaTime / period;
            yield return null;
        }
        if (OnComplete != null)
        {
            OnComplete();
        }
        Destroy(gameObject);
    }
}
