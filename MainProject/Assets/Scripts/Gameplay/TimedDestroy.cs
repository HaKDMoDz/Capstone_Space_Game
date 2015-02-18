using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimedDestroy : MonoBehaviour 
{
    [SerializeField]
    private float timer = 1.0f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }

}
