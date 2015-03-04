using UnityEngine;
using System.Collections;

public class InvasionGUIManager : MonoBehaviour 
{
    [SerializeField]
    public FillBar fillbar;

	void Start () 
    {
        fillbar.SetValue(0.0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
