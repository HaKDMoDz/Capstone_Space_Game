using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

public class ShipDesignTester : MonoBehaviour
{
#if TESTING
    [SerializeField]
    Transform hullPlacementLoc;
    [SerializeField]
    HullTable hullTableObj;
    [SerializeField]
    ComponentTable compTableObj;

    Dictionary<int, Hull> hullTable;
    Dictionary<int, ShipComponent> compTable;

    IEnumerator Start()
    {
        hullTable = hullTableObj.HullTableProp
          .ToDictionary(h => h.ID, h => h.hull);
        compTable = compTableObj.ComponentList
            .ToDictionary(c => c.ID, c => c.component);





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
        Debug.LogWarning("Test NoSlotsFilled complete");
        
        foreach (var hullEntry in hullTable)
        {
            print("Running FillUpWithSameComp Tests for " + hullEntry.Value.name);
            yield return StartCoroutine(FillUpWithSameComp(hullEntry.Key));
        }
        Debug.LogWarning("Test FillUpWithSameComp complete");

        foreach (var hullEntry in hullTable)
        {
            print("Running RandomCompsAllSlots Tests for " + hullEntry.Value.name);
            yield return StartCoroutine(RandomCompsAllSlots(hullEntry.Key, 5));
        }
        Debug.LogWarning("Test RandomCompsAllSlots complete");

    }

    IEnumerator NoSlotsFilled(int hullID)
    {
        print("Running Test: NoSlotsFilled");

        ShipDesignSystem.Instance.BuildHull(hullID);
        string fileName = "Test_NoSlotsFilled_Hull_" + hullTable[hullID].name;
        ShipDesignSystem.Instance.DeleteBlueprint(fileName);
        ShipDesignSystem.Instance.SaveBlueprint(fileName);
        ShipDesignSystem.Instance.ClearBlueprint();
        yield return new WaitForSeconds(0.2f);
        ShipDesignSystem.Instance.LoadBlueprint(fileName);
        ShipBlueprint loadedBP = typeof(ShipDesignSystem).GetField("currentBlueprint", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(ShipDesignSystem.Instance) as ShipBlueprint;
        int _hullID = hullTable.FirstOrDefault(h => h.Value == loadedBP.Hull).Key;
        if (_hullID == hullID && loadedBP.ComponentTable.Count == 0)
        {
            Debug.Log("Loaded Blueprint matches");
        }
        else
        {
            Debug.LogError("Error in Test: NoSlotsFilled - Loaded blueprint does not match ");
            Debug.Break();
        }
        ShipDesignSystem.Instance.DeleteBlueprint(fileName);
        ShipDesignSystem.Instance.ClearBlueprint();
        yield return null;
    }
    IEnumerator FillUpWithSameComp(int hullID)
    {
        print("Running Test: FillUpWithSameComp");
        Dictionary<int, int> slotVerificationTable = new Dictionary<int, int>();
        MethodInfo AddCompMethod = typeof(ShipDesignSystem).GetMethod("AddCompToDisplay", BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var compEntry in compTable)
        {
            print("Testing " + compEntry.Value.componentName);
            slotVerificationTable.Clear();
            ShipDesignSystem.Instance.BuildHull(hullID);
            Hull buildingHull = typeof(ShipDesignSystem).GetField("currentHull", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(ShipDesignSystem.Instance) as Hull;
            
            ShipBlueprint shipBP = typeof(ShipDesignSystem).GetField("currentBlueprint", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(ShipDesignSystem.Instance) as ShipBlueprint;
            foreach (var slotEntry in buildingHull.SlotTable)
            {
                AddCompMethod.Invoke(ShipDesignSystem.Instance, new object[] { slotEntry.Value, compEntry.Value });
                shipBP.AddComponent(compEntry.Value, slotEntry.Value);
                slotVerificationTable.Add(slotEntry.Key, compEntry.Key);
                yield return null;
            }
            StringBuilder sb = new StringBuilder("Test_FillUpWithSameComp_Hull_");
            sb.Append(hullTable[hullID].name);
            sb.Append("_Comp_");
            sb.Append(compEntry.Value.componentName);
            string fileName = sb.ToString();
            
            ShipDesignSystem.Instance.DeleteBlueprint(fileName);
            ShipDesignSystem.Instance.SaveBlueprint(fileName);
            ShipDesignSystem.Instance.ClearBlueprint();
            yield return new WaitForSeconds(0.2f);
            ShipDesignSystem.Instance.LoadBlueprint(fileName);
            ShipBlueprint loadedBP = typeof(ShipDesignSystem).GetField("currentBlueprint", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(ShipDesignSystem.Instance) as ShipBlueprint;
            yield return new WaitForSeconds(0.2f);
            if (!VerifyLoadedBlueprint(loadedBP, slotVerificationTable, hullID))
            {
                Debug.LogError("Error in Test: FillUpWithSameComp - Loaded blueprint does not match ");
                Debug.Break();
            }
            else
            {
                Debug.Log("Loaded Blueprint matches");
            }
            ShipDesignSystem.Instance.DeleteBlueprint(fileName);
            ShipDesignSystem.Instance.ClearBlueprint();
        }
    }
    IEnumerator RandomCompsAllSlots(int hullID, int numTests)
    {
        print("Running Test: RandomCompsAllSlots");
        int[] compIDs = compTable.Keys.ToArray();
        Dictionary<int, int> slotVerificationTable = new Dictionary<int, int>();
        MethodInfo AddCompMethod = typeof(ShipDesignSystem).GetMethod("AddCompToDisplay", BindingFlags.NonPublic | BindingFlags.Instance);

        for (int i = 0; i < numTests; i++)
        {
            slotVerificationTable.Clear();
            ShipDesignSystem.Instance.BuildHull(hullID);
            Hull buildingHull = typeof(ShipDesignSystem).GetField("currentHull", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(ShipDesignSystem.Instance) as Hull;
            
            ShipBlueprint shipBP = typeof(ShipDesignSystem).GetField("currentBlueprint", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(ShipDesignSystem.Instance) as ShipBlueprint;
            foreach (var slotEntry in buildingHull.SlotTable)
            {
                int compID = compIDs[Random.Range(0, compIDs.Length)];
                AddCompMethod.Invoke(ShipDesignSystem.Instance, new object[] { slotEntry.Value, compTable[compID] });
                shipBP.AddComponent(compTable[compID], slotEntry.Value);
                slotVerificationTable.Add(slotEntry.Key, compID);
                yield return null;
            }
            string fileName = "Test_RandomCompsAllSlots_Hull" + hullTable[hullID].name;
            ShipDesignSystem.Instance.DeleteBlueprint(fileName);
            ShipDesignSystem.Instance.SaveBlueprint(fileName);
            ShipDesignSystem.Instance.ClearBlueprint();
            yield return new WaitForSeconds(0.2f);
            ShipDesignSystem.Instance.LoadBlueprint(fileName);
            ShipBlueprint loadedBP = typeof(ShipDesignSystem).GetField("currentBlueprint", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(ShipDesignSystem.Instance) as ShipBlueprint;
            yield return new WaitForSeconds(0.2f);
            if (!VerifyLoadedBlueprint(loadedBP, slotVerificationTable, hullID))
            {
                Debug.LogError("Error in Test: FillUpWithSameComp - Loaded blueprint does not match ");
                Debug.Break();
            }
            else
            {
                Debug.Log("Loaded Blueprint matches");
            }
            ShipDesignSystem.Instance.DeleteBlueprint(fileName);
            ShipDesignSystem.Instance.ClearBlueprint();
        }
    }
    
    bool VerifyLoadedBlueprint(ShipBlueprint loadedBlueprint, Dictionary<int, int> slotVerificationTable, int _hullID)
    {
        int hullID = hullTable.FirstOrDefault(h => h.Value == loadedBlueprint.Hull).Key;
        if (hullID != _hullID)
        {
            return false;
        }
        if (loadedBlueprint.ComponentTable.Count != slotVerificationTable.Count)
        {
            return false;
        }
        for (int i = 0; i < slotVerificationTable.Count; i++)
        {
            int compID = compTable.FirstOrDefault(c => c.Value == loadedBlueprint.ComponentTable.ElementAt(i).Value).Key;
            if (compID != slotVerificationTable.ElementAt(i).Value
                || loadedBlueprint.ComponentTable.ElementAt(i).Key.index != slotVerificationTable.ElementAt(i).Key)
            {
                //print("compID " + compID + " ver " + slotVerificationTable.ElementAt(i).Value);
                return false;
            }
        }
        return true;
    }
    //IEnumerator ResetScreen()
    //{
    //    currentBlueprint = null;
    //    if (currentHull)
    //    {
    //        Destroy(currentHull.gameObject);
    //    }
    //    else
    //    {
    //        print("no current Hull found");
    //    }
    //    if (componentsDisplayed == null)
    //    {
    //        componentsDisplayed = new List<ShipComponent>();
    //        slotDisplayedObjectTable = new Dictionary<ComponentSlot, ShipComponent>();
    //    }
    //    else
    //    {
    //        for (int i = 0; i < componentsDisplayed.Count; i++)
    //        {
    //            Destroy(componentsDisplayed[i].gameObject);
    //        }
    //        componentsDisplayed.Clear();
    //        slotDisplayedObjectTable.Clear();
    //    }
    //    yield return null;
    //}

    //IEnumerator BuildHull(int hullID)
    //{
    //    //print("build hull " + hullTable[hullID]);
    //    yield return StartCoroutine(AddHullToDisplay(hullTable[hullID]));
    //    currentBlueprint = new ShipBlueprint(hullTable[hullID]);
    //}
    //IEnumerator AddHullToDisplay(Hull hull)
    //{
    //    currentHull = Instantiate(hull, hullPlacementLoc.position, hull.transform.rotation) as Hull;
    //    currentHull.Init();
    //    CameraManager.Instance.HullDisplayed(hull);
    //    yield return null;
    //}
    //IEnumerator BuildComponent(int compID, ComponentSlot slot)
    //{
    //    if (slot.installedComponent)
    //    {
    //        ShipComponent otherComp = slotDisplayedObjectTable[slot];
    //        componentsDisplayed.Remove(otherComp);
    //        Destroy(otherComp.gameObject);
    //        currentBlueprint.RemoveComponent(slot);
    //    }
    //    yield return StartCoroutine(AddCompToDisplay(slot, compTable[compID]));
    //    currentBlueprint.AddComponent(compTable[compID], slot);
    //}
    //IEnumerator AddCompToDisplay(ComponentSlot slot, ShipComponent component)
    //{

    //    ShipComponent builtComp = Instantiate(component, slot.transform.position, slot.transform.rotation) as ShipComponent;
    //    componentsDisplayed.Add(builtComp);
    //    if (slotDisplayedObjectTable.ContainsKey(slot))
    //    {
    //        slotDisplayedObjectTable[slot] = builtComp;
    //    }
    //    else
    //    {
    //        slotDisplayedObjectTable.Add(slot, builtComp);
    //    }
    //    yield return null;
    //}
    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 100, Screen.height - 100, 100, 100), "<size=24><color=red>DEBUG BUILD</color></size>");
    }
#endif
}
