using UnityEngine;
using System.Collections;

public class AICube : MonoBehaviour 
{
    public float movementEpsilon = 0.2f;
    
    bool moveLeft = true;

    Transform _trans;

    void Start()
    {
        _trans = transform;
    }
    public IEnumerator StartTurn()
    {
        Debug.Log("AI Start Turn");

        yield return StartCoroutine(Move());
    }
    IEnumerator Move()
    {
        Vector3 dest;
        if(moveLeft)
        {
            dest = _trans.position+Vector3.left*5f;
        }
        else
        {
            dest = _trans.position+Vector3.right*5f;
        }
        Vector3 dir = dest - _trans.position; 
        while (Vector3.SqrMagnitude(dir) > movementEpsilon * movementEpsilon)
        {
            _trans.Translate(dir * Time.deltaTime);
            dir = dest - _trans.position;
            yield return null;
        }
        moveLeft = !moveLeft;
    }

}
