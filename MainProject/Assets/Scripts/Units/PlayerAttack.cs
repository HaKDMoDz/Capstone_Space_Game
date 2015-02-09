using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PlayerAttack : MonoBehaviour 
{
    #region Fields

    //internal references
    private Transform trans;
    private AI_Ship targetShip;
    private ShipComponent targetComponent;
    public LineRenderer line;

    //helper
    private bool targetConfirmed = false;
    private int targetShipIndex;
    private int numAiShips;
    private int numWeaponsActivated;
    
    #endregion Fields

    #region Methods

    #region PublicMethods
    
    public void Init()
    {
        trans = transform;
    }
    /// <summary>
    /// Activates the selected components and raises the ActivationComplete event with the amount of power consumed as a parameter.
    /// Currently only handles logic to activate weapons.
    /// </summary>
    /// <param name="componentsToActivate"></param>
    /// <param name="activationComplete"></param>
    /// <returns></returns>
    public IEnumerator ActivateComponents(List<ShipComponent> componentsToActivate, Action<float> activationComplete)
    {
        float totalPowerUsed = 0.0f; //used to keep track of the power used in case all the selected components are not able to successfully activate

        //if there are any weapons in the selection
        if(componentsToActivate.Any(c=>c is Component_Weapon))
        {
            yield return StartCoroutine(WeaponTargetingSequence());

            numWeaponsActivated = 0; //keeps tracks of the callbacks from the activated weapons to know when all the weapons are done firing

            if(targetComponent)
            {
                trans.LookAt(targetComponent.transform);

                //Activates each weapon in turn
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
        //waits until all weapons have completes their animation
        while(numWeaponsActivated>0)
        {
            yield return null;
        }

        //raises the event with the power consumed by the components who managed to activate successfully
        activationComplete(totalPowerUsed);

        //removes the targeting panel once weapon activation is complete
        TargetShip(targetShip, false);
        //focuses the camera back on the ship
        yield return StartCoroutine(CameraDirector.Instance.MoveToFocusOn(trans, GlobalVars.CameraMoveToFocusPeriod));


    }//ActivateComponents

    #endregion PublicMethods

    #region PrivateMethods
    
    //Waits for the player to select the target ship and target component to fire the selected weapons at
    private IEnumerator WeaponTargetingSequence()
    {

        targetShipIndex = 0;
        targetConfirmed = false;
        targetComponent = null;

        List<AI_Ship> ai_ships = TurnBasedCombatSystem.Instance.ai_Ships;
        numAiShips = ai_ships.Count;
        targetShip = ai_ships[targetShipIndex];

        #if UNITY_EDITOR
        Debug.Log("Select Target to fire upon: [Click] to confirm, [Esc] to cancel, [Tab] to switch targets");
        #endif
        
        #if !NO_DEBUG
        if (numAiShips == 0)
        {
            Debug.LogError("No ai ships found");
        }
        #endif
        //aims camera at the ship that iscurrently targeted 
        yield return StartCoroutine(CameraDirector.Instance.AimAtTarget(trans, ai_ships[targetShipIndex].transform, GlobalVars.CameraAimAtPeriod));
        //shows the targeting panel for the target ship
        TargetShip(ai_ships[targetShipIndex], true);

        //runs until a targetcomponent is successfully confirmed (until a component is clicked on)
        while(!targetConfirmed)
        {
            //end targeting sequence upon hitting Esc
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TargetShip(ai_ships[targetShipIndex], false);
                targetConfirmed = true;
                targetComponent = null;
            }
            //switch to the next ai ship 
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                TargetShip(ai_ships[targetShipIndex], false);
                targetShipIndex = ++targetShipIndex % numAiShips;
                targetShip = ai_ships[targetShipIndex];
                //Debug.Log(targetShipIndex);
                TargetShip(ai_ships[targetShipIndex], true);
                yield return StartCoroutine(CameraDirector.Instance.AimAtTarget(trans, ai_ships[targetShipIndex].transform, GlobalVars.CameraAimAtPeriod));
            }
            yield return null;

        }//while

    }//WeaponTargetingSequence

    /// <summary>
    /// Shows a line to indicate line of fire and shows the target ship's targeting panel for the player to select a component to target
    /// </summary>
    /// <param name="targetUnit"></param>
    /// <param name="show"></param>
    private void TargetShip(TurnBasedUnit targetUnit, bool show)
    {
        if(!targetShip)
        {
            return;
        }
        if (!show)
        {
            DisplayTargetingLine(Vector3.zero, false); //hide line
            targetUnit.ShowTargetingPanel(false); //hide panel
            //unsubscribe to component callbacks
            foreach (ShipComponent component in targetUnit.Components)
            {
                component.OnComponentClicked -= OnComponentClick;
                component.OnComponentMouseOver -= OnComponentMouseOver;
                
            }
        }
        else
        {
            targetUnit.ShowTargetingPanel(true);
            foreach (ShipComponent component in targetUnit.Components)
            {
                component.OnComponentClicked += OnComponentClick;
                component.OnComponentMouseOver += OnComponentMouseOver;
                component.OnComponentPointerExit += OnComponentPointerExit;
            }
        }
    }
    /// <summary>
    /// The pointer leaves a component. Target component is de-selected.
    /// </summary>
    /// <param name="component"></param>
    private void OnComponentPointerExit(ShipComponent component)
    {
        if (targetComponent)
        {
            targetComponent.Selected = false;
        }
        component.Selected = false;
    }
    /// <summary>
    /// Mouse over a component. The first component in the direction of the component moused over is selected
    /// </summary>
    /// <param name="component"></param>
    private void OnComponentMouseOver(ShipComponent component)
    {
        //Debug.Log("Targeted component " + component.componentName);
        if (targetComponent)
        {
            targetComponent.Selected = false;
        }
        targetComponent = GetFirstCompInDirection(component);
        DisplayTargetingLine(targetComponent.transform.position, true);
        targetComponent.Selected = true;
    }
    /// <summary>
    /// Click on a component. The first component in the direction of the component clicked on is selected and confirmed as the target to fire weapons at
    /// </summary>
    /// <param name="component"></param>
    private void OnComponentClick(ShipComponent component)
    {
        //Debug.Log("Selected target: " + component.componentName);
        if (targetComponent)
        {
            targetComponent.Selected = false;
        }
        targetComponent = GetFirstCompInDirection(component);
        targetComponent.Selected = true;
        targetConfirmed = true;
    }
    private ShipComponent GetFirstCompInDirection(ShipComponent component)
    {
        Ray ray = new Ray(trans.position, component.transform.position - trans.position);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, GlobalVars.RayCastRange, 1<<TagsAndLayers.ComponentsLayer))
        {
            return hit.collider.GetComponent<ShipComponent>();
        }
        return component;
    }
    private void DisplayTargetingLine(Vector3 targetPos, bool show)
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
