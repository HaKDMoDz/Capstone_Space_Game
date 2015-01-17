using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipMove : MonoBehaviour
{
    #region Fields
    public Vector3 destination { get; set; }

    private Transform trans;
    #endregion Fields

    #region Methods
    public void Init()
    {
        trans = transform;
    }
    public IEnumerator Move()
    {
        Debug.Log("moving to " + destination);
        Vector3 moveDir = destination - trans.position;
        trans.LookAt(destination);
        yield return null;
    }
    #endregion Methods
}
