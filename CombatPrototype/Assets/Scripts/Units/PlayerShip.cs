﻿using UnityEngine;
using System.Collections;

public class PlayerShip : TurnBasedUnit
{


    //cached components
    ShipMove shipMove;
    ShipAttack shipAttack;

    public override void Awake()
    {
        base.Awake();
        shipMove = gameObject.GetSafeComponent<ShipMove>();
        shipAttack = gameObject.GetSafeComponent<ShipAttack>();
    }


    public override IEnumerator ExecuteTurn()
    {
        Debug.Log(unitName + " (PlayerShip) starts turn");
        projector.enabled = true;

        yield return base.ExecuteTurn();

        //ends the player's turn only upon hitting Space
        while (!Input.GetKeyDown(KeyCode.Space))
        {

            if (Input.GetMouseButtonDown(1))
            {
                yield return StartCoroutine(shipMove.Move(GetWorldCoordsFromMouse(Input.mousePosition)));
            }
            //else if(Input.GetMouseButtonDown(0))
            else if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                yield return StartCoroutine(shipAttack.FireLasers(GetWorldCoordsFromMouse(Input.mousePosition)));
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                yield return StartCoroutine(shipAttack.FireMissiles(GetWorldCoordsFromMouse(Input.mousePosition)));
            }
            
            
            yield return null;
        }

        projector.enabled = false;
        Debug.Log(unitName + " (PlayerShip) ends turn");
    }

    

    Vector3 GetWorldCoordsFromMouse(Vector3 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0f));
        RaycastHit hit;

        Vector3 worldCoords = trans.position;

        if (Physics.Raycast(ray, out hit, 1000f, 1 << GlobalConstants.Instance.groundLayer))
        {
            worldCoords = hit.point;
            worldCoords.y = trans.position.y;
        }
        return worldCoords;
    }

}
