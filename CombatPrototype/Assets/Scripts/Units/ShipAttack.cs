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

    //List<Comp_Weapon_Laser> lasers;
    //List<Comp_Weapon_Missile> missiles;

    //cached components
    Transform trans;
    ShipBlueprint shipBlueprint;

    //book-keeping vars
    bool activationComplete=false;
    AIShip targetShip;

    public void Init()
    {
        trans = transform;
        shipBlueprint = gameObject.GetSafeComponent<ShipBlueprint>();
        //lasers = shipBlueprint.weapons.OfType<Comp_Weapon_Laser>().ToList();
        //missiles = shipBlueprint.weapons.OfType<Comp_Weapon_Missile>().ToList();
    }

    public IEnumerator ActivateComponents(List<ShipComponent> components)
    {
        activationComplete = false;
        targetShip = null;

        if(components[0] is Component_Weapon)
        {
            yield return StartCoroutine(WeaponTargetSelectionSequence(components));
           // for (int i = 0; i < lasers.Count; i++)
    //    {
    //        lasers[i].Fire(shootPoint,selectedTarget, ActivationComplete);
    //        yield return new WaitForSeconds(0.1f);
    //    }
            if (targetShip)
            {
                trans.LookAt(targetShip.transform);
                foreach (Component_Weapon weapon in components)
                {
                    Debug.Log("Firing on: " + targetShip.gameObject.name);
                    weapon.Fire(targetShip.transform, ActivationComplete);
                }
            }
        }
        else
        {
            Debug.Log("not weapons");
            activationComplete = true;
        }
        //foreach (ShipComponent comp in components)
        //{
        //    comp.Activate(ActivationComplete);
        //}

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


    //public IEnumerator Fire(Vector3 aimPos)
    //{
    //    activationComplete = false;
        
    //    trans.LookAt(aimPos);
    //    Transform selectedTarget = GameObject.FindObjectOfType<AIShip>().transform;

    //    for (int i = 0; i < lasers.Count; i++)
    //    {
    //        lasers[i].Fire(shootPoint,selectedTarget, ActivationComplete);
    //        yield return new WaitForSeconds(0.1f);
    //    }


    //    while (!activationComplete)
    //    {
    //        yield return null;
    //    }

    //}

    public void ActivationComplete()
    {
        activationComplete = true;
    }

    //public IEnumerator FireMissiles(Vector3 aimPos)
    //{
    //    Vector3 shootDir = aimPos - trans.position;
    //    trans.LookAt(aimPos);
    //    shootDir.Normalize();
    //    GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation) as GameObject;
    //    projectile.rigidbody.AddForce(shootDir * 5000f);
    //    yield return new WaitForSeconds(1f);
    //}
}
