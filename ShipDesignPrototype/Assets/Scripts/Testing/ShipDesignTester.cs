using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShipDesignTester : MonoBehaviour
{
    [SerializeField]
    Transform hullPlacementLoc;
    [SerializeField]
    HullTable hullTableObj;
    [SerializeField]
    ComponentTable compTableObj;

    Dictionary<int, Hull> hullTable;
    Dictionary<int, ShipComponent> compTable;

    ShipBlueprint currentBlueprint, verificationBlueprint;
    Hull currentHull;
    List<ShipComponent> componentsDisplayed;
    Dictionary<ComponentSlot, ShipComponent> slotDisplayedObjectTable;

    IEnumerator Start()
    {
        hullTable = hullTableObj.HullTableProp
          .ToDictionary(h => h.ID, h => h.hull);
        compTable = compTableObj.ComponentList
            .ToDictionary(c => c.ID, c => c.component);
        componentsDisplayed = new List<ShipComponent>();
        slotDisplayedObjectTable = new Dictionary<ComponentSlot, ShipComponent>();
        yield return StartCoroutine(RunTests());
        Debug.LogWarning("Tests Complete");
    }

    IEnumerator RunTests()
    {
        Debug.LogWarning("Running tests");

        foreach (var hullEntry in hullTable)
        {
            yield return StartCoroutine(RunTestsForHull(hullEntry.Key));
        }
    }
    IEnumerator RunTestsForHull(int hullID)
    {
        print("Running tests for " + hullTable[hullID].name);
        //go through all tests for the hull - various permutations of components
        yield return StartCoroutine(BuildHull(hullID));
        yield return StartCoroutine(FillOneSlot());
    }
    IEnumerator FillOneSlot()
    {
        print("Running Test: Fill one slot");
        //loop through all comps
        //loop through all slots
        //install comp
        //save
        //clear
        //load
        //verify
        foreach (var compEntry in compTable)
        {
            foreach (var slotEntry in currentHull.SlotTable)
            {

                BuildComponent(compEntry.Key, slotEntry.Value);
                string fileName = "Test_Comp_" + compEntry.Key + "_Slot_" + slotEntry.Key;
                ShipBlueprintSaveSystem.Instance.DeleteBlueprint(fileName);
                ShipBlueprintSaveSystem.Instance.Save(currentBlueprint, fileName);
                verificationBlueprint = currentBlueprint;
                yield return StartCoroutine(ResetScreen());
                if (ShipBlueprintSaveSystem.Instance.Load(out currentBlueprint, fileName))
                {
                    yield return StartCoroutine(AddHullToDisplay(currentBlueprint.Hull));

                    foreach (var item in currentBlueprint.ComponentTable)
                    {
                        yield return StartCoroutine(AddCompToDisplay(item.Key, item.Value));
                    }
                }
                else
                {
                    Debug.LogError("Error in Test: Fill one slot - Could not find file " + fileName);
                }
                if (!ValidateLoadedBlueprint(currentBlueprint))
                {
                    Debug.LogError("Error in Test: Fill one slot - Loaded blueprint does not match ");
                }
                ShipBlueprintSaveSystem.Instance.DeleteBlueprint(fileName);
                yield return null;
            }
        }
        
        yield return StartCoroutine(ResetScreen());
    }
    bool ValidateLoadedBlueprint(ShipBlueprint loadedBlueprint)
    {
        if (loadedBlueprint.Hull != verificationBlueprint.Hull)
        { 
            return false; 
        }
        if (loadedBlueprint.ComponentTable.Count != verificationBlueprint.ComponentTable.Count)
        { 
            return false; 
        }

        for (int i = 0; i < loadedBlueprint.ComponentTable.Count; i++)
        {
            if (loadedBlueprint.ComponentTable.ElementAt(i).Key != verificationBlueprint.ComponentTable.ElementAt(i).Key
                || loadedBlueprint.ComponentTable.ElementAt(i).Value != verificationBlueprint.ComponentTable.ElementAt(i).Value)
            { 
                return false; 
            }
        }
        return true;
    }
    IEnumerator ResetScreen()
    {
        currentBlueprint = null;
        if (currentHull)
        {
            //Debug.Log("Destroying hull");
            Destroy(currentHull.gameObject);
        }
        currentHull = null;
        if (componentsDisplayed == null)
        {
            componentsDisplayed = new List<ShipComponent>();
            slotDisplayedObjectTable = new Dictionary<ComponentSlot, ShipComponent>();
            //Debug.Log(componentsDisplayed.Count);
        }
        else
        {
            for (int i = 0; i < componentsDisplayed.Count; i++)
            {
                Destroy(componentsDisplayed[i].gameObject);
                yield return null; 
            }
            componentsDisplayed.Clear();
            slotDisplayedObjectTable.Clear();
        }
        yield return null;
    }
    IEnumerator FillUpWithSameComp()
    {
        print("Running Test: FillUpWithSameComp");
        yield return null;
    }
    IEnumerator FillUpSequentialComps()
    {
        print("Running Test: FillUpSequentialComps");
        yield return null;
    }
    IEnumerator NoSlotsFilled()
    {
        print("Running Test: NoSlotsFilled");
        yield return null;
    }
    IEnumerator RandomCompsAllSlots()
    {
        print("Running Test: RandomCompsAllSlots");
        yield return null;
    }
    IEnumerator RandomCompsRandomSlots()
    {
        print("Running Test: RandomCompsRandomSlots");
        yield return null;
    }


    IEnumerator BuildHull(int hullID)
    {
        print("build hull " + hullTable[hullID]);
        yield return StartCoroutine(AddHullToDisplay(hullTable[hullID]));
        currentBlueprint = new ShipBlueprint(hullTable[hullID]);

    }
    IEnumerator AddHullToDisplay(Hull hull)
    {
        currentHull = Instantiate(hull, hullPlacementLoc.position, hull.transform.rotation) as Hull;
        currentHull.Init();
        CameraManager.Instance.HullDisplayed(hull);
        yield return null;
    }
    IEnumerator BuildComponent(int compID, ComponentSlot slot)
    {
        AddCompToDisplay(slot, compTable[compID]);
        yield return null;
    }
    IEnumerator AddCompToDisplay(ComponentSlot slot, ShipComponent component)
    {
        ShipComponent builtComp = Instantiate(component, slot.transform.position, slot.transform.rotation) as ShipComponent;
        componentsDisplayed.Add(builtComp);
        if (slotDisplayedObjectTable.ContainsKey(slot))
        {
            slotDisplayedObjectTable[slot] = builtComp;
        }
        else
        {
            slotDisplayedObjectTable.Add(slot, builtComp);
        }
        yield return null;
    }
    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 100, Screen.height - 100, 100, 100), "<size=24><color=red>DEBUG BUILD</color></size>");
    }
}
