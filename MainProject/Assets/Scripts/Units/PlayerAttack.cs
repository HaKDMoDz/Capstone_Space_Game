using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerAttack : MonoBehaviour 
{
    #region Fields
    #region Internal

    private Transform trans;
    bool targetConfirmed = false;
    AI_Ship targetShip;
    int targetShipIndex;
    int numAiShips;
    int numWeaponsActivated;
    
    #endregion Internal
    #endregion Fields

    #region Methods

    #region PublicMethods
    
    public void Init()
    {
        trans = transform;

    }

    public IEnumerator ActivateComponents(List<ShipComponent> components)
    {
        //if there are any weapons in the selection
        if(components.Any(c=>c is Component_Weapon))
        {
            yield return StartCoroutine(WeaponTargetingSequence());
            numWeaponsActivated = 0;

            if(targetShip)
            {
                Transform targetShipTrans = targetShip.transform;
                trans.LookAt(targetShipTrans);

                foreach (Component_Weapon weapon in components.Where(c => c is Component_Weapon))
                {
                    Debug.Log("activate weapon");
                    
                    yield return StartCoroutine(
                    weapon.Fire(targetShipTrans,
                        () =>
                        {
                            numWeaponsActivated--;
                        }));
                    numWeaponsActivated++;
                }
            }
            #if !NO_DEBUG
            else
            {
                Debug.Log("Targeting Sequence Complete");
            }
            #endif
        }

        while(numWeaponsActivated>0)
        {
            yield return null;
        }

        yield return StartCoroutine(CameraDirector.Instance.MoveToFocusOn(trans, GlobalVars.CameraMoveToFocusPeriod));


    }//ActivateComponents

    #endregion PublicMethods

    #region PrivateMethods
    
    private IEnumerator WeaponTargetingSequence()
    {
        targetShipIndex = 0;
        targetConfirmed = false;
        targetShip = null;

        List<AI_Ship> ai_ships = TurnBasedCombatSystem.Instance.ai_Ships;
        numAiShips = ai_ships.Count;

        #if UNITY_EDITOR
        Debug.Log("Select Target to fire upon: [Click] to confirm, [Esc] to cancel, [Tab] to switch targets");
        #endif
        
        #if !NO_DEBUG
        if (numAiShips == 0)
        {
            Debug.LogError("No ai ships found");
        }
        #endif
        
        yield return StartCoroutine(CameraDirector.Instance.AimAtTarget(trans, ai_ships[targetShipIndex].transform, GlobalVars.CameraAimAtPeriod));

        while(!targetConfirmed)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                targetConfirmed = true;
                targetShip = null;
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                targetShipIndex = ++targetShipIndex % numAiShips;
                Debug.Log(targetShipIndex);
                yield return StartCoroutine(CameraDirector.Instance.AimAtTarget(trans, ai_ships[targetShipIndex].transform, GlobalVars.CameraAimAtPeriod));
            }
            if(Input.GetMouseButtonDown((int)MouseButton.Left))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 1000.0f, 1<<TagsAndLayers.AI_ShipLayer))
                {
                    Debug.Log(hit.collider.gameObject.name);
                    targetConfirmed = true;
                    targetShip = hit.collider.gameObject.GetComponent<AI_Ship>();
                }
            }
            yield return null;
        }//while

    }//WeaponTargetingSequence

    #endregion PrivateMethods

    #endregion Methods
}
