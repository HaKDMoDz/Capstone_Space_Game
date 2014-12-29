using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class ShipDesignTester : MonoBehaviour
{

#if UNITY_EDITOR

    #region Fields
    #region EditorExposed
    [SerializeField]
    private bool fastTest=true;
    #endregion EditorExposed
    #region Internal
    private Dictionary<int, Hull> id_hull_table;
    private Dictionary<Hull, int> hull_id_table;
    private Dictionary<int, ShipComponent> id_comp_table;
    private Dictionary<ShipComponent, int> comp_id_table;

    private Dictionary<int, int> slotIndex_compID_table;
    #endregion Internal
    #endregion Fields

    #region Methods
    #region Private
    #region UnityCallBacks
    private IEnumerator Start()
    {
        id_hull_table = ShipDesignSystem.Instance.id_hull_table;
        hull_id_table = ShipDesignSystem.Instance.hull_id_table;
        id_comp_table = ShipDesignSystem.Instance.id_comp_table;
        comp_id_table = ShipDesignSystem.Instance.comp_id_table;

        slotIndex_compID_table = new Dictionary<int, int>();

        yield return StartCoroutine(RunTests());
        Debug.Log("Tests Complete");
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 300, Screen.height - 100, 300, 100), "<size=28><color=red>TESTING BUILD</color></size>");
    }
    #endregion UnityCallBacks

    #region Tests
    private IEnumerator RunTests()
    {
        Debug.LogWarning("Running tests");

        //foreach (var id_hull in HullTable.ID_Hull_Table)
        foreach (var id_hull in id_hull_table)
        {
            Debug.Log("Running EmptyHull test for" + id_hull.Value.hullName);
            yield return StartCoroutine(EmptyHull(id_hull.Key));
        }
        Debug.LogWarning("EmptyHull tests complete");

        foreach (var id_hull in id_hull_table)
        {
            Debug.Log("Running FillWithSameComponents test for" + id_hull.Value.hullName);
            yield return StartCoroutine(FilledWithSameComponent(id_hull.Key));
        }
        Debug.LogWarning("FillWithSameComponents tests complete");

        foreach (var id_hull in id_hull_table)
        {
            Debug.Log("Running FillWithRandomComponents test for" + id_hull.Value.hullName);
            yield return StartCoroutine(FillWithRandomComponents(id_hull.Key, 5));
        }
        Debug.LogWarning("FillWithRandomComponents tests complete");

    }
    private IEnumerator EmptyHull(int hull_ID)
    {
        ShipDesignSystem.Instance.BuildHull(hull_ID);
        //string fileName = "Test_EmptyHull_" + HullTable.GetHull(hull_ID).hullName;
        string fileName = "Test_EmptyHull_" + id_hull_table[hull_ID].hullName;
        //ShipDesignSystem.Instance.DeleteBlueprint(fileName);
        ShipDesignSystem.Instance.SaveBlueprint(fileName);
        
        yield return new WaitForSeconds(0.2f);
        
        ShipDesignSystem.Instance.ClearScreen();
        //design.load
        ShipDesignSystem.Instance.LoadBlueprint(fileName);
        ShipBlueprint loadedBP = typeof(ShipDesignSystem)
            .GetField("blueprintBeingBuilt", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(ShipDesignSystem.Instance) as ShipBlueprint;
        int loadedHull_ID = hull_id_table[loadedBP.hull];
        if (loadedHull_ID == hull_ID && loadedBP.slot_component_table.Count == 0)
        {
            Debug.Log("Loaded Blueprint matches");
        }
        else
        {
            Debug.LogError("Error in Test: EmptyHull - Loaded blueprint does not match ");
            Debug.Break();
        }
        ShipDesignSystem.Instance.DeleteBlueprint(fileName);
        yield return new WaitForSeconds(0.2f);
        ShipDesignSystem.Instance.ClearScreen();

    }
    private IEnumerator FilledWithSameComponent(int hull_ID)
    {
        //foreach (var id_comp in ComponentTable.id_comp_table)
        foreach (var id_comp in id_comp_table)
        {
            Debug.Log("Testing " + id_comp.Value.componentName);
            slotIndex_compID_table.Clear();
            ShipDesignSystem.Instance.BuildHull(hull_ID);
            Hull hullBeingBuilt = typeof(ShipDesignSystem)
                .GetField("hullBeingBuilt", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(ShipDesignSystem.Instance)
                as Hull;
            foreach (ComponentSlot slot in hullBeingBuilt.EmptyComponentGrid)
            {
                //add component (slot, component)
                //slotindextable.add
                ShipDesignSystem.Instance.BuildComponent(slot, id_comp.Value);
                slotIndex_compID_table.Add(slot.index, id_comp.Key);
                if (!fastTest)
                {
                    yield return null;
                }
            }

            //string fileName = "Test_FilledWithSameComponent_Hull_" + HullTable.GetHull(hull_ID).hullName + "_Comp_" + id_comp.Value.componentName;
            string fileName = "Test_FilledWithSameComponent_Hull_" + id_hull_table[hull_ID].hullName + "_Comp_" + id_comp.Value.componentName;
            //ShipDesignSystem.Instance.DeleteBlueprint(fileName);
            ShipDesignSystem.Instance.SaveBlueprint(fileName);
            ShipDesignSystem.Instance.ClearScreen();

            yield return new WaitForSeconds(0.2f);

            //design.load
            ShipDesignSystem.Instance.LoadBlueprint(fileName);

            //get loadedBP
            ShipBlueprint loadedBP = typeof(ShipDesignSystem)
            .GetField("blueprintBeingBuilt", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(ShipDesignSystem.Instance) as ShipBlueprint;

            if(ValidateLoadedBlueprint(loadedBP, slotIndex_compID_table, hull_ID))
            {
                Debug.Log("Loaded Blueprint matches");
            }
            else
            {
                Debug.LogError("Error in Test: FilledWithSameComponent - Loaded blueprint does not match ");
                Debug.Break();
            }
            
            yield return new WaitForSeconds(0.2f);

            ShipDesignSystem.Instance.DeleteBlueprint(fileName);
            ShipDesignSystem.Instance.ClearScreen();
        }
    }
    private IEnumerator FillWithRandomComponents(int hull_ID, int numTests)
    {
        //int[] compIDs = ComponentTable.id_comp_table.Keys.ToArray();
        int[] compIDs = id_comp_table.Keys.ToArray();

        for (int i = 0; i < numTests; i++)
        {
            slotIndex_compID_table.Clear();
            ShipDesignSystem.Instance.BuildHull(hull_ID);
            Hull hullBeingBuilt = typeof(ShipDesignSystem)
                .GetField("hullBeingBuilt", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(ShipDesignSystem.Instance) as Hull;

            foreach (ComponentSlot slot in hullBeingBuilt.EmptyComponentGrid)
            {
                int randCompID = compIDs[Random.Range(0, compIDs.Length)];
                //ShipDesignSystem.Instance.BuildComponent(slot, ComponentTable.GetComponent(randCompID));
                ShipDesignSystem.Instance.BuildComponent(slot, id_comp_table[randCompID]);
                slotIndex_compID_table.Add(slot.index, randCompID);
                if (!fastTest)
                {
                    yield return null;
                }
            }
            //string fileName = "Test_FillWithRandomComponents_Hull" + HullTable.GetHull(hull_ID).hullName;
            string fileName = "Test_FillWithRandomComponents_Hull" + id_hull_table[hull_ID].hullName;
            //ShipDesignSystem.Instance.DeleteBlueprint(fileName);
            ShipDesignSystem.Instance.SaveBlueprint(fileName);
            ShipDesignSystem.Instance.ClearScreen();

            yield return new WaitForSeconds(0.2f);

            ShipDesignSystem.Instance.LoadBlueprint(fileName);
            ShipBlueprint loadedBP = typeof(ShipDesignSystem)
            .GetField("blueprintBeingBuilt", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(ShipDesignSystem.Instance) as ShipBlueprint;
            if(ValidateLoadedBlueprint(loadedBP, slotIndex_compID_table, hull_ID))
            {
                Debug.Log("Loaded Blueprint matches");
            }
            else
            {
                Debug.LogError("Error in Test: FillWithRandomComponents - Loaded blueprint does not match ");
                Debug.Break();
            }
            yield return new WaitForSeconds(0.2f);

            ShipDesignSystem.Instance.DeleteBlueprint(fileName);
            ShipDesignSystem.Instance.ClearScreen();

        }


    }
    #endregion Tests

    #region Helper
    bool ValidateLoadedBlueprint(ShipBlueprint loadedBP, Dictionary<int, int> correct_slotIndex_compID_table, int correct_hull_ID)
    {
        //if (HullTable.GetID(loadedBP.hull) != correct_hull_ID)
        if (hull_id_table[loadedBP.hull] != correct_hull_ID)
        {
            return false;
        }
        if (loadedBP.slot_component_table.Count != correct_slotIndex_compID_table.Count)
        {
            return false;
        }
        foreach (var slot_component in loadedBP.slot_component_table)
        {
            int slotIndex = slot_component.Key.index;
            if(!correct_slotIndex_compID_table.ContainsKey(slotIndex))
            {
                return false;
            }
            //int loadedCompID =  ComponentTable.GetID(slot_component.Value);
            int loadedCompID = comp_id_table[slot_component.Value];
            int correctCompID = correct_slotIndex_compID_table[slotIndex];
            if(loadedCompID!=correctCompID )
            {
                return false;
            }
        }
        return true;
    }
    #endregion Helper
    #endregion Private
    #endregion Methods

#endif

}
