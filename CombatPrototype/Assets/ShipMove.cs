using UnityEngine;
using System.Collections;
using System;



public class ShipMove : MonoBehaviour 
{

    public float moveSpeed;
    public float turnSpeed;

    [SerializeField]
    float movementEpsilon;

    //cached components
    Transform trans;

    void Awake()
    {
        trans = transform;
    }

    public IEnumerator Move(Vector3 destination)
    {

        Vector3 moveDir = destination - trans.position;

        while(Vector3.SqrMagnitude(moveDir) > movementEpsilon*movementEpsilon)
        {
            trans.Translate(moveDir * moveSpeed * Time.deltaTime);
            moveDir = destination - transform.position;
            yield return null;

        }

    }


}
