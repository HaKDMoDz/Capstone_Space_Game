using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogOfWar : MonoBehaviour 
{
    [SerializeField]
    ShipMove shipMove;
    [SerializeField]
    int fogOfWarLayer;

    Material mat;

    void Start()
    {
        mat = renderer.material;
        shipMove.OnShipMoved += shipMove_OnShipMoved;
    }

    void shipMove_OnShipMoved(Transform trans)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(trans.position);
        Ray rayToShip = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;
        if(Physics.Raycast(rayToShip, out hit, 100f, 1<<fogOfWarLayer ))
        {
            mat.SetVector("_ShipPos", hit.point);
        }
    }

}
