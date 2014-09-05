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
        
        while (Vector3.SqrMagnitude(moveDir) > movementEpsilon * movementEpsilon)
        {
            trans.position = Vector3.Lerp(trans.position, destination, moveSpeed * Time.deltaTime);
            
            StartCoroutine(CameraDirector.Instance.FocusOn(trans, .1f));
            moveDir = destination - trans.position;
            yield return null;

        }
        Debug.Log("Movement End");

    }


}
