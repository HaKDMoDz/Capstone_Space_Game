using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShipStatsPanel : MonoBehaviour
{
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private string blueprintName;
    public string BlueprintName
    {
        get { return blueprintName; }
        set
        {
            blueprintName = value;
            nameText.text = blueprintName;
        }
    }

    [SerializeField]
    private Text excessPowerText;
    [SerializeField]
    private float excessPower;
    public float ExcessPower
    {
        get { return excessPower; }
        set
        {
            excessPower = value;
            excessPowerText.text = excessPower.ToString();
        }
    }

    public void UpdateStats(string blueprintName, float excessPower)
    {
        BlueprintName = blueprintName;
        ExcessPower = excessPower;
    }

}
