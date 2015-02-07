using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PlayerAttack : MonoBehaviour 
{
    #region Fields
    #region Internal

    private Transform trans;
    //private AI_Ship targetShip;
    private ShipComponent targetComponent;

    private bool targetConfirmed = false;
    private int targetShipIndex;
    private int numAiShips;
    private int numWeaponsActivated;

    public LineRenderer line;
    
    #endregion Internal
    #endregion Fields

    #region Methods

    #region PublicMethods
    
    public void Init()
    {
        trans = transform;
    }

    public IEnumerator ActivateComponents(List<ShipComponent> componentsToActivate, Action<float> activationComplete)
    {
        float totalPowerUsed = 0.0f;
        //if there are any weapons in the selection
        if(componentsToActivate.Any(c=>c is Component_Weapon))
        {
            yield return StartCoroutine(WeaponTargetingSequence());
            numWeaponsActivated = 0;

            if(targetComponent)
            {
                //Transform targetShipTrans = targetShip.transform;
                //trans.LookAt(targetShipTrans);
                //Transform targetCompTrans = targetComponent.transform;
                trans.LookAt(targetComponent.transform);

                foreach (Component_Weapon weapon in componentsToActivate.Where(c => c is Component_Weapon))
                {
                    if (targetComponent && targetComponent.CompHP > 0.0f)
                    {
                        Debug.Log("activate weapon");
                        yield return StartCoroutine(
                        weapon.Fire(targetComponent,
                            () =>
                            {
                                numWeaponsActivated--;
                                totalPowerUsed += weapon.ActivationCost;
                            }));
                        numWeaponsActivated++;
                    }
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
        activationComplete(totalPowerUsed);
        yield return StartCoroutine(CameraDirector.Instance.MoveToFocusOn(trans, GlobalVars.CameraMoveToFocusPeriod));


    }//ActivateComponents

    #endregion PublicMethods

    #region PrivateMethods
    
    private IEnumerator WeaponTargetingSequence()
    {
        targetShipIndex = 0;
        targetConfirmed = false;
        //targetShip = null;
        targetComponent = null;

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
        TargetShip(ai_ships[targetShipIndex], true);

        while(!targetConfirmed)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TargetShip(ai_ships[targetShipIndex], false);
                targetConfirmed = true;
                //targetShip = null;
                targetComponent = null;
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                TargetShip(ai_ships[targetShipIndex], false);
                targetShipIndex = ++targetShipIndex % numAiShips;
                //Debug.Log(targetShipIndex);
                TargetShip(ai_ships[targetShipIndex], true);

                yield return StartCoroutine(CameraDirector.Instance.AimAtTarget(trans, ai_ships[targetShipIndex].transform, GlobalVars.CameraAimAtPeriod));
            }
            //if(Input.GetMouseButtonDown((int)MouseButton.Left))
            //{
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    RaycastHit hit;
            //    if(Physics.Raycast(ray, out hit, 1000.0f, 1<<TagsAndLayers.AI_ShipLayer))
            //    {
            //        Debug.Log(hit.collider.gameObject.name);
            //        targetConfirmed = true;
            //        targetShip = hit.collider.gameObject.GetComponent<AI_Ship>();
            //    }
            //}
            yield return null;

        }//while

        TargetShip(ai_ships[targetShipIndex], false);

    }//WeaponTargetingSequence

    private void TargetShip(TurnBasedUnit targetUnit, bool show)
    {
        if (!show)
        {
            DisplayTargetingLine(Vector3.zero, false);
            targetUnit.ShowTargetingPanel(false);
            foreach (ShipComponent component in targetUnit.Components)
            {
                component.OnComponentClicked -= OnComponentClick;
                component.OnComponentMouseOver -= OnComponentMouseOver;
                
            }
        }
        else
        {
            //DisplayTargetingLine(targetUnit.transform.position, true);
            targetUnit.ShowTargetingPanel(true);
            foreach (ShipComponent component in targetUnit.Components)
            {
                component.OnComponentClicked += OnComponentClick;
                component.OnComponentMouseOver += OnComponentMouseOver;
                component.OnComponentPointerExit += OnComponentPointerExit;
            }
        }
    }

    void OnComponentPointerExit(ShipComponent component)
    {
        component.Selected = false;
    }
    void OnComponentMouseOver(ShipComponent component)
    {
        Debug.Log("Targeted component " + component.componentName);
        DisplayTargetingLine(component.transform.position, true);
        component.Selected = true;
    }

    void OnComponentClick(ShipComponent component)
    {
        Debug.Log("Selected target: " + component.componentName);
        component.Selected = true;
        targetConfirmed = true;
        targetComponent = component;
    }

    void DisplayTargetingLine(Vector3 targetPos, bool show)
    {
        line.enabled = show;
        if(!show)
        {
            return;
        }

        Vector3 targetDir = targetPos - trans.position;
        int lineLength = Mathf.RoundToInt(targetDir.magnitude)+1;
        targetDir.Normalize();

        line.SetVertexCount(lineLength);

        for (int i = 0; i < lineLength; i++)
        {
            Vector3 newPos = trans.position;
            Vector3 offset = Vector3.zero;
            offset.x = newPos.x + i * targetDir.x;
            offset.y = newPos.y + i * targetDir.y;
            offset.z = newPos.z + i * targetDir.z;
            newPos = offset;
            line.SetPosition(i, newPos);
        }
    }


    #endregion PrivateMethods

    #endregion Methods
}
