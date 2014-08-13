using UnityEngine;
using System.Collections;

public class GUI_EditorInfo : MonoBehaviour {

	// Use this for initialization
	bool shiftHeld;
	void Start () {
	

	}

	void OnGUI()
	{
		GUI.color = Color.yellow;
		GUI.Box (new Rect (10, 10, 230, 25), "Use mouse to control camera and aim");
		GUI.Box (new Rect (10, 40, 150, 25), "Left Shift Add/Remove");

		if (shiftHeld) {
				GUI.color = Color.red;
				GUI.Box (new Rect (10, 70, 100, 25), "Remove");
		}
		else {
			GUI.color = Color.green;
			GUI.Box (new Rect (10, 70, 100, 25), "Add");
		}
					

	}
	
	// Update is called once per frame
	void Update () {
		shiftHeld = Input.GetKey(KeyCode.LeftShift);
		
	}
}
