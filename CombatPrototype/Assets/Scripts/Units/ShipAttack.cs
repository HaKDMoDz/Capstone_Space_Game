using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class ShipAttack : MonoBehaviour
{
    
    //cached components
    Transform trans;
    //ShipBlueprint shipBlueprint;

    //book-keeping vars
    bool activationComplete=false;
    AIShip targetShip;


    public void Init()
    {
        trans = transform;
        //shipBlueprint = gameObject.GetSafeComponent<ShipBlueprint>();
    }

    public IEnumerator ActivateComponents(List<ShipComponent> components)
    {
        activationComplete = false;
        targetShip = null;

        if(components[0] is Component_Weapon)
        {
            yield return StartCoroutine(WeaponTargetSelectionSequence(components));

            if (targetShip)
            {
                trans.LookAt(targetShip.transform);
                foreach (Component_Weapon weapon in components)
                {
                    Debug.Log("Firing on: " + targetShip.gameObject.name);
                    weapon.Fire(targetShip.transform, 
                        ()=>{
                            activationComplete = true;
                            targetShip.TakeDamage(weapon.damage);
                        });
                }


            }
        }
        else
        {
            Debug.Log("not weapons");
            activationComplete = true;
        }
    
        while (!activationComplete)
        {
            yield return null;
        }
        yield return StartCoroutine(CameraDirector.Instance.FocusOn(trans, 1.0f));
    }

    IEnumerator WeaponTargetSelectionSequence(List<ShipComponent> components)
    {
        Debug.Log("Select Target to fire upon: [Click] to confirm, [Esc] to cancel");
        List<AIShip> aiShips = TurnBasedCombatSystem.Instance.aiShips;
        int numAIShips = aiShips.Count;
        if (numAIShips > 0)
        {
            bool targetConfirmed = false;
            int targetShipIndex = 0;
            yield return StartCoroutine(CameraDirector.Instance.AimAtTarget(trans, aiShips[targetShipIndex].transform, 1.0f));

            while (!targetConfirmed)
            {
                
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    targetConfirmed = true;
                    targetShip = null;
                }
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    targetShipIndex = ++targetShipIndex % numAIShips;
                    Debug.Log(targetShipIndex);
                    yield return StartCoroutine(CameraDirector.Instance.AimAtTarget(trans, aiShips[targetShipIndex].transform, 1.0f));
                }
                if (Input.GetMouseButton(2))
                {
                    CameraDirector.Instance.OrbitAroundImmediate(trans, Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                }
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 1000f, 1 << GlobalTagsAndLayers.Instance.layers.enemyShipLayer))
                    {
                        Debug.Log("Clicked on: " + hit.collider.gameObject.name);
                        targetConfirmed = true;
                        targetShip = hit.collider.gameObject.GetSafeComponent<AIShip>();
                    }
                }

                yield return null;
            }
            

        }
        else
        {
            Debug.Log("No Targets available");
        }
    }
    


    
}
