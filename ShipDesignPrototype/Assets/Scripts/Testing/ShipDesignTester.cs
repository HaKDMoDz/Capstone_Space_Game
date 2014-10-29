using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    ShipBlueprint currentBlueprint;//, verificationBlueprint;
    Hull currentHull;
    List<ShipComponent> componentsDisplayed;
    Dictionary<ComponentSlot, ShipComponent> slotDisplayedObjectTable;

    //bool displayTest = true;

    IEnumerator Start()
    {
        hullTable = hullTableObj.HullTableProp
          .ToDictionary(h => h.ID, h => h.hull);
        compTable = compTableObj.ComponentList
            .ToDictionary(c => c.ID, c => c.component);
        //if (displayTest)
        {
            componentsDisplayed = new List<ShipComponent>();
            slotDisplayedObjectTable = new Dictionary<ComponentSlot, ShipComponent>();
        }
        yield return StartCoroutine(RunTests());
        Debug.LogWarning("Tests Complete");
    }

    IEnumerator RunTests()
    {
        Debug.LogWarning("Running tests");

        foreach (var hullEntry in hullTable)
        {
            print("Running NoSlotsFilled Tests for " + hullEntry.Value.name);
            yield return StartCoroutine(NoSlotsFilled(hullEntry.Key));
        }
        yield return StartCoroutine(ResetScreen());
        //ResetScreen();
        Debug.LogWarning("Test NoSlotsFilled complete");
        //Debug.Break();
        foreach (var hullEntry in hullTable)
        {
            print("Running FillUpWithSameComp Tests for " + hullEntry.Value.name);
            yield return StartCoroutine(FillUpWithSameComp(hullEntry.Key));
        }
        Debug.LogWarning("Test FillUpWithSameComp complete");
    }
    IEnumerator RunTestsForHull(int hullID)
    {
        
        //go through all tests for the hull - various permutations of components
        //yield return StartCoroutine(BuildHull(hullID));
        //if (displayTest)
        {
            //yield return StartCoroutine(FillOneSlot(hullID));
            yield return StartCoroutine(NoSlotsFilled(hullID));
            
        }
    }
    IEnumerator FillOneSlot(int hullID)
    {
        print("Running Test: Fill one slot");
        foreach (var compEntry in compTable)
        {
           //foreach (var slotEntry in currentHull.SlotTable)
            Dictionary<int, ComponentSlot> slotTable = currentHull.SlotTable;

            for (int i = 0; i < slotTable.Count; i++)
            {
                //if (displayTest)
                {
                    yield return StartCoroutine(BuildComponent(compEntry.Key, slotTable.ElementAt(i).Value));
                }
                //currentBlueprint.AddComponent(slotEntry.Key, compEntry.Value);

                string fileName = "Test_Comp_" + compEntry.Key + "_Slot_" + slotTable.ElementAt(i).Key;
                ShipBlueprintSaveSystem.Instance.DeleteBlueprint(fileName);
                ShipBlueprintSaveSystem.Instance.Save(currentBlueprint, fileName);
                //verificationBlueprint = currentBlueprint;
                yield return StartCoroutine(ResetScreen());
                //ResetScreen();
                if (ShipBlueprintSaveSystem.Instance.Load(out currentBlueprint, fileName))
                {
                    Debug.Log("load");
                    yield return StartCoroutine(AddHullToDisplay(currentBlueprint.Hull));
                    Debug.Log("hull added");
                    foreach (var item in currentBlueprint.ComponentTable)
                    {
                        yield return StartCoroutine(AddCompToDisplay(item.Key, item.Value));
                    }
                    Debug.Log("comps added");
                }
                else
                {
                    Debug.LogError("Error in Test: Fill one slot - Could not find file " + fileName);
                }
               // if (!VerifyLoadedBlueprint(currentBlueprint))
                {
                    Debug.LogError("Error in Test: Fill one slot - Loaded blueprint does not match ");
                }
                ShipBlueprintSaveSystem.Instance.DeleteBlueprint(fileName);
                yield return null;
            }
        }

        yield return StartCoroutine(ResetScreen());
        //ResetScreen();
    }
  
    IEnumerator FillUpWithSameComp(int hullID)
    {
        print("Running Test: FillUpWithSameComp");
        Dictionary<int, int> slotVerificationTable = new Dictionary<int, int>();
        
        foreach (var compEntry in compTable)
        {
            print("Testing " + compEntry.Value.componentName);
            slotVerificationTable.Clear();
            yield return StartCoroutine(BuildHull(hullID));
            //foreach (ComponentSlot slot in currentHull.SlotTable.Values)
            foreach (var slotEntry in currentHull.SlotTable)
            {
                yield return StartCoroutine(BuildComponent(compEntry.Key, slotEntry.Value));
                slotVerificationTable.Add(slotEntry.Key, compEntry.Key);
            }
            StringBuilder sb = new StringBuilder("Test_AllFilledWithOne_Hull");
            sb.Append(hullTable[hullID].name);
            sb.Append("_Comp_");
            sb.Append(compEntry.Value);
            string fileName = sb.ToString();
            ShipBlueprintSaveSystem.Instance.DeleteBlueprint(fileName);
            ShipBlueprintSaveSystem.Instance.Save(currentBlueprint, fileName);
            //verificationBlueprint = new ShipBlueprint(currentBlueprint);
            yield return StartCoroutine(ResetScreen());
            //ResetScreen();
            if(ShipBlueprintSaveSystem.Instance.Load(out currentBlueprint, fileName))
            {
                yield return StartCoroutine(AddHullToDisplay(currentBlueprint.Hull));
                foreach (var item in currentBlueprint.ComponentTable)
                {
                    yield return StartCoroutine(AddCompToDisplay(item.Key, item.Value));
                }
            }
            else
            {
                Debug.LogError("Error in Test: FillUpWithSameComp - Could not find file " + fileName);
                Debug.Break();
            }
            
            if (!VerifyLoadedBlueprint(currentBlueprint, slotVerificationTable, hullID))
            {
                Debug.LogError("Error in Test: FillUpWithSameComp - Loaded blueprint does not match ");
                Debug.Break();
            }
            else
            {
                Debug.Log("Loaded Blueprint matches");
            }
            ShipBlueprintSaveSystem.Instance.DeleteBlueprint(fileName);
            yield return StartCoroutine(ResetScreen());
        }
    }
    IEnumerator FillUpSequentialComps(int hullID)
    {
        print("Running Test: FillUpSequentialComps");

        yield return null;
    }
    IEnumerator NoSlotsFilled(int hullID)
    {
        print("Running Test: NoSlotsFilled");
        yield return StartCoroutine(BuildHull(hullID));
        string fileName = "Test_NoSlotsFilled_Hull" + hullTable[hullID].name;
        ShipBlueprintSaveSystem.Instance.DeleteBlueprint(fileName);
        ShipBlueprintSaveSystem.Instance.Save(currentBlueprint, fileName);
        yield return StartCoroutine(ResetScreen());
        //ResetScreen();
        //Debug.Break();
        for (int i = 0; i < 100000; i++)
        {

        }
        if(ShipBlueprintSaveSystem.Instance.Load(out currentBlueprint, fileName))
        {
            yield return StartCoroutine(AddHullToDisplay(currentBlueprint.Hull));
            foreach (var item in currentBlueprint.ComponentTable)
            {
                yield return StartCoroutine(AddCompToDisplay(item.Key, item.Value));
            }
        }
        else
        {
            Debug.LogError("Error in Test: NoSlotsFilled - Could not find file " + fileName);
            Debug.Break();
        }
        int _hullID = hullTable.FirstOrDefault(h=>h.Value==currentBlueprint.Hull).Key;
        if(_hullID == hullID && currentBlueprint.ComponentTable.Count==0)
        {
            Debug.Log("Loaded Blueprint matches");
        }
        else
        {
            Debug.LogError("Error in Test: NoSlotsFilled - Loaded blueprint does not match ");
            Debug.Break();
        }
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
    bool VerifyLoadedBlueprint(ShipBlueprint loadedBlueprint, Dictionary<int, int> slotVerificationTable, int _hullID)
    {
        //var myValue = types.FirstOrDefault(x => x.Value == "one").Key;
        int hullID = hullTable.FirstOrDefault(h=>h.Value==loadedBlueprint.Hull).Key;
        if(hullID!=_hullID)
        {
            return false;
        }
        if(loadedBlueprint.ComponentTable.Count != slotVerificationTable.Count)
        {
            return false;
        }
        for (int i = 0; i < slotVerificationTable.Count; i++)
        {
            int compID = compTable.FirstOrDefault(c=>c.Value == loadedBlueprint.ComponentTable.ElementAt(i).Value).Key;
            if(compID != slotVerificationTable.ElementAt(i).Value
                || loadedBlueprint.ComponentTable.ElementAt(i).Key.index != slotVerificationTable.ElementAt(i).Key)
            {
                print("did not match");
                //print("compID " + compID + " ver " + slotVerificationTable.ElementAt(i).Value);
                return false;
            }
        }
        return true;
    }
    //bool VerifyLoadedBlueprint(ShipBlueprint loadedBlueprint)
    //{
    //    if (loadedBlueprint.Hull != verificationBlueprint.Hull)
    //    { 
    //        return false; 
    //    }
    //    if (loadedBlueprint.ComponentTable.Count != verificationBlueprint.ComponentTable.Count)
    //    { 
    //        return false; 
    //    }
    //    for (int i = 0; i < loadedBlueprint.ComponentTable.Count; i++)
    //    {
    //        print(loadedBlueprint.ComponentTable.ElementAt(i).Key + "----" + verificationBlueprint.ComponentTable.ElementAt(i).Key);
    //        if (loadedBlueprint.ComponentTable.ElementAt(i).Key != verificationBlueprint.ComponentTable.ElementAt(i).Key)
    //            //|| loadedBlueprint.ComponentTable.ElementAt(i).Value != verificationBlueprint.ComponentTable.ElementAt(i).Value)
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //}
    IEnumerator ResetScreen()
    //void ResetScreen()
    {
        //print("reset screen");
        currentBlueprint = null;
        if (currentHull)
        {
            //Debug.Log("Destroying hull "+currentHull);
            Destroy(currentHull);
            //DestroyImmediate(currentHull);
        }
        else
        {
            print("no current Hull found");
        }
        //currentHull = null;
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
                //yield return null; 
            }
            componentsDisplayed.Clear();
            slotDisplayedObjectTable.Clear();
        }
        yield return null;
    }
   


    IEnumerator BuildHull(int hullID)
    {
        //print("build hull " + hullTable[hullID]);
        yield return StartCoroutine(AddHullToDisplay(hullTable[hullID]));
        //currentHull.Init();
        currentBlueprint = new ShipBlueprint(hullTable[hullID]);
        //if(currentHull.SlotTable == null)
        //{
        //    print("slottable null");
        //    Debug.Break();
        //}
    }
    IEnumerator AddHullToDisplay(Hull hull)
    {
        //if (displayTest)
        {
            currentHull = Instantiate(hull, hullPlacementLoc.position, hull.transform.rotation) as Hull;
            currentHull.Init();
            CameraManager.Instance.HullDisplayed(hull);
        }
        yield return null;
    }
    IEnumerator BuildComponent(int compID, ComponentSlot slot)
    {
        if(slot.installedComponent)
        {
            ShipComponent otherComp = slotDisplayedObjectTable[slot];
            componentsDisplayed.Remove(otherComp);
            Destroy(otherComp.gameObject);
            currentBlueprint.RemoveComponent(slot);
        }
        yield return StartCoroutine(AddCompToDisplay(slot, compTable[compID]));
        currentBlueprint.AddComponent(compTable[compID],slot);
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
