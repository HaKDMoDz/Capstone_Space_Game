using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipDesignInterface : Singleton<ShipDesignInterface>
{

    #region Fields
    #endregion Fields

    #region Methods
    #region Private
    #region UnityCallbacks
    private void Start()
    {
        SetupGUI();
    }
    #endregion UnityCallbacks
    #region GUI
    private void SetupGUI()
    {

    }
    #endregion GUI
    #endregion Private
    
    #region Public
    #region GUI

    #endregion GUI

    #region DesignSystemAccess
    public void BuildHull(int hull_ID)
    {
        ShipDesignSystem.Instance.BuildHull(hull_ID);
    }
    public void BuildComponent(ComponentSlot slot, ShipComponent component)
    {
        ShipDesignSystem.Instance.BuildComponent(slot, component);
    }
    public void SaveBlueprint(string fileName)
    {
        ShipDesignSystem.Instance.SaveBlueprint(fileName);
    }
    public void LoadBlueprint(string fileName)
    {
        ShipDesignSystem.Instance.LoadBlueprint(fileName);
    }
    public void DeleteBlueprint(string fileName)
    {
        ShipDesignSystem.Instance.DeleteBlueprint(fileName);
    }
    public void DeleteAllBlueprints()
    {
        ShipDesignSystem.Instance.DeleteAllBlueprints();
    }
    #endregion DesignSystemAccess
    #endregion Public
    #endregion Methods

}
