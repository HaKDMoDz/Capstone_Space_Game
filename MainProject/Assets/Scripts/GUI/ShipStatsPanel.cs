using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShipStatsPanel : MonoBehaviour
{
    [SerializeField]
    private InputField inputField;
    [SerializeField]
    private Text excessPowerText;

    
    private float excessPower=0.0f;
    public float ExcessPower
    {
        get { return excessPower; }
        set
        {
            excessPower = value;
            excessPowerText.text = excessPower.ToString();
        }
    }

    public string GetBlueprintName()
    {
        return inputField.text;
    }
    public void SetBlueprintName(string bpName)
    {
        inputField.text = bpName;
    }
    public void UpdateStats(string blueprintName, float excessPower)
    {
        SetBlueprintName(blueprintName);
        ExcessPower = excessPower;
    }

}
