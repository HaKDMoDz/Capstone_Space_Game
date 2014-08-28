using UnityEngine;
using System.Collections;

public class ColorState : MonoBehaviour {

	public bool Selected = false;
	public Color SelectedColor;
	public Color IdleColor;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Selected)
			renderer.material.color = SelectedColor;
		else 
			renderer.material.color = IdleColor;
		Selected = false;
	}
}
