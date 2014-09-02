using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class ShipAttack : MonoBehaviour
{
    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    Transform shootPoint;

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
    }

    IEnumerator WeaponTargetSelectionSequence(List<ShipComponent> components)
    {
        Debug.Log("Select Target to fire upon: [Click] to confirm, [Esc] to cancel");
        bool targetConfirmed = false;

        while(!targetConfirmed)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                targetConfirmed = true;
                targetShip = null;
            }
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 1000f, 1<<GlobalTagsAndLayers.Instance.layers.enemyShipLayer))
                {
                    Debug.Log("Clicked on: " + hit.collider.gameObject.name);
                    targetConfirmed = true;
                    targetShip = hit.collider.gameObject.GetSafeComponent<AIShip>();
                }
            }

            yield return null;
        }


    }
    


    
}
