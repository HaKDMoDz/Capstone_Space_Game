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
    [SerializeField]
    private Text thrustText;
    
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
    private float thrust = 0.0f;
    public float Thrust
    {
        get { return thrust; }
        set 
        { 
            thrust = value;
            thrustText.text = thrust.ToString("0.000");
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
    public void UpdateStats(string blueprintName, float excessPower, float thrust)
    {
        SetBlueprintName(blueprintName);
        ExcessPower = excessPower;
        Thrust = thrust;
    }

}
