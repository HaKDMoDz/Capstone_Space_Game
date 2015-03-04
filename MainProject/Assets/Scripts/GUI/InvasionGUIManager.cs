using UnityEngine;
using System.Collections;

public class InvasionGUIManager : Singleton<InvasionGUIManager> 
{
    [SerializeField]
    public FillBar fillbar;

	void Start () 
    {
        //fillbar.SetValue(0.0f);
        //setValue(0);
        
        int _completedPlanets = 0;

        foreach (bool completeStatus in GameController.Instance.GameData.galaxyMapData.completeStatus)
        {
            if (completeStatus)
            {
                _completedPlanets++;
            }
        }

        setValue(_completedPlanets);
	}
	
	public void setValue(int _value)
    {
        //error validation. force values to 0 - 10
        if (_value < 0)
        {
            _value = 0;
        }

        if (_value > 10)
        {
            _value = 10; 
        }

        fillbar.SetValue((float)_value / 10.0f);
    }
}
