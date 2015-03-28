/*
  ShipStatsPanel.cs
  Mission: Invasion
  Created by Rohun Banerji on Jan 14/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

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
    [SerializeField]
    private Text shieldText;
    
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
    private float shieldStr = 0.0f;
    public float ShieldStr
    {
        get { return shieldStr; }
        set 
        { 
            shieldStr = value;
            shieldText.text = shieldStr.ToString();
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
    public void UpdateStats(string blueprintName, float excessPower, float thrust, float shieldStr)
    {
        SetBlueprintName(blueprintName);
        ExcessPower = excessPower;
        excessPowerText.color = ExcessPower <= 0.0f ? Color.red : Color.white;
        Thrust = thrust;
        thrustText.color = Thrust <= 0.0f ? Color.red : Color.white;
        ShieldStr = shieldStr;
        shieldText.color = ShieldStr > 0.0f ? Color.white : Color.red;
    }

}
