using UnityEngine;
using System.Collections;

public class Editor_MouseControl : MonoBehaviour {

	public float MouseRotationSensitivity =1;
	public float MaxZoom =100;
	public float MinZoom =1;

	public float VerticalRotationMax = 90;

	Vector3 previousMousePos;

	float HorizontalRotation;
	float VerticalRotation;
	float zpos;
	// Use this for initialization
	void Start () {
		previousMousePos = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(1))previousMousePos = Input.mousePosition;
		if (Input.GetMouseButton (1)) 
		{
			Vector3 difference = previousMousePos - Input.mousePosition;

			HorizontalRotation += -difference.x * MouseRotationSensitivity;
			VerticalRotation += difference.y * MouseRotationSensitivity;
			VerticalRotation = Mathf.Clamp(VerticalRotation, -VerticalRotationMax, VerticalRotationMax);

			//Debug.Log("Camera Rotation: Horizontal: "+HorizontalRotation+" Vertical: "+VerticalRotation);

			transform.rotation = Quaternion.Euler(new Vector3(VerticalRotation, 
                                                              transform.rotation.eulerAngles.y,
                                                              transform.rotation.eulerAngles.z));

            transform.parent.rotation = Quaternion.Euler(new Vector3(transform.parent.rotation.eulerAngles.x,
			                                                         HorizontalRotation,
			                                                         transform.parent.rotation.eulerAngles.z));        
		

			previousMousePos = Input.mousePosition;
		}

	}
}
