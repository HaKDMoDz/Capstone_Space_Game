using UnityEngine;
using System.Collections;
using System;



public class ShipMove : MonoBehaviour
{

    public float moveSpeed;
    public float turnSpeed;

    [SerializeField]
    float movementEpsilon = 0.2f;

    //cached components
    Transform trans;

    public void Init()
    {
        trans = transform;
    }

    public IEnumerator Move(Vector3 destination)
    {

        Vector3 moveDir = destination - trans.position;
        trans.LookAt(destination);
        //trans.rotation = Quaternion.LookRotation(moveDir);


        while (Vector3.SqrMagnitude(moveDir) > movementEpsilon * movementEpsilon)
        {
            //trans.Translate(moveDir * moveSpeed * Time.deltaTime);
            //trans.Translate(trans.forward * moveSpeed * Time.deltaTime);
            //trans.Translate(0f, 0f, moveSpeed/10);
            //trans.SetPositionZ(trans.position.+moveSpeed * Time.deltaTime);
            //trans.position = Vector3.MoveTowards(trans.position, destination, moveSpeed*Time.deltaTime);
            trans.position = Vector3.Lerp(trans.position, destination, moveSpeed * Time.deltaTime);
            moveDir = destination - trans.position;
            yield return null;

        }
        Debug.Log("Movement End");

    }


}
