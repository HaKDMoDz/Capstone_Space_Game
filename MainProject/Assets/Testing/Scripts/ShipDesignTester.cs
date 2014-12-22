using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipDesignTester : MonoBehaviour 
{

#if UNITY_EDITOR

    #region Fields
    #region Internal
    private Dictionary<int, Hull> id_hull_table;
    private Dictionary<int, ShipComponent> id_comp_table;
    #endregion Internal
    #endregion Fields

    #region Methods
    #region Private
    #region UnityCallBacks
    private IEnumerator Start()
    {
        id_hull_table = ShipDesignSystem.Instance.id_hull_table;
        id_comp_table = ShipDesignSystem.Instance.id_comp_table;
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

        foreach (var hullEntry in id_hull_table)
        {
            Debug.Log("Running EmptyHull test for" + hullEntry.Value.hullName);
            yield return StartCoroutine(EmptyHull(hullEntry.Key));
        }
        Debug.LogWarning("EmptyHull tests complete");

        foreach (var hullEntry in id_hull_table)
        {
            Debug.Log("Running FillWithSameComponents test for" + hullEntry.Value.hullName);
            yield return StartCoroutine(FilledWithSameComponent(hullEntry.Key));
        }
        Debug.LogWarning("FillWithSameComponents tests complete");

        foreach (var hullEntry in id_hull_table)
        {
            Debug.Log("Running FillWithRandomComponents test for" + hullEntry.Value.hullName);
            yield return StartCoroutine(FillWithRandomComponents(hullEntry.Key));
        }
        Debug.LogWarning("FillWithRandomComponents tests complete");

    }
    private IEnumerator EmptyHull(int hull_ID)
    {
        //design.build hull (id)
        ShipDesignSystem.Instance.BuildHull(hull_ID);
        //filename = 
        //design.deleteBP
        //design.saveBP
        //design.clearScreen
        yield return new WaitForSeconds(0.4f);
        ShipDesignSystem.Instance.ClearScreen();

        //design.load
        //get loadedBP
        //verify
        //design.deleteBP
        //design.clear

    }
    private IEnumerator FilledWithSameComponent(int hullID)
    {
        Dictionary<int, int> slotIndex_compID_table = new Dictionary<int, int>();
        foreach (var compEntry in id_comp_table)
        {
            Debug.Log("Testing " + compEntry.Value.componentName);
            slotIndex_compID_table.Clear();
            //design.buildHull(hullID)
            //get building hull
            //get current BP

            //for buildingHull.SlotTable
                //add component (slot, component)
                //slotindextable.add

            //get filename
            //design.delete, SaveBP
            //design.clear

            yield return new WaitForSeconds(0.2f);

            //design.load
            //get loadedBP
            yield return new WaitForSeconds(0.2f);

            //verify
            //design.delete
            //design.clear
        }

        yield return null;
    }
    private IEnumerator FillWithRandomComponents(int hullID)
    {
        yield return null;
    }
    private bool VerifyLoadedBlueprint()
    {
        return false;
    }

    #endregion Tests
    #endregion Private
    #endregion Methods

#endif

}
