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
        //Debug.Log("moving to " + destination);
        Vector3 moveDir = destination - trans.position;
        trans.LookAt(destination);
        while (Vector3.SqrMagnitude(moveDir) > GlobalVars.MovementEpsilon * GlobalVars.MovementEpsilon)
        {
            trans.position = Vector3.Lerp(trans.position, destination, GlobalVars.PlayerShipMoveSpeed * Time.deltaTime);
            StartCoroutine(CameraDirector.Instance.MoveToFocusOn(trans, GlobalVars.CameraFollowPeriod));
            moveDir = destination - trans.position;
            yield return null;
        }
    }
    #endregion Methods
}
