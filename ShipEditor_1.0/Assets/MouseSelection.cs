using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseSelection : MonoBehaviour {

	public Camera camera;
	public int ComponentSlotID;
	Ray mouseRay;
	KeyCode Remove = KeyCode.LeftShift;
	ShipLayout layout;
	List<GameObject> ComponentButtons;
	RaycastHit hit;
	GameObject SelectedComponent;
	int SelectedComponentID = -1;
	Vector3 offset;
	bool FinishedRemoving = false;




	// Use this for initialization
	void Start () {
		layout = GetComponent<ShipLayout>();
		offset = new Vector3(0,0.5f,0);
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetMouseButton(0))
		{
			mouseRay = new Ray(camera.ScreenToWorldPoint(Input.mousePosition), camera.transform.forward);

			if(Physics.Raycast(mouseRay,out hit, 50f))
			{
				if(!CheckGrid())//if nothing on the grid is selected then check the buttons
				CheckButtons();
			}
		}
		if(Input.GetMouseButtonUp(0))FinishedRemoving = true;
	}
	bool CheckGrid()
	{
		if(hit.transform.gameObject.name == "GridTile(Clone)")
		{
			hit.transform.gameObject.GetComponent<ColorState>().Selected = true;
			if(SelectedComponentID == ComponentSlotID)
			{
				if(!Input.GetKey(Remove))
				{
					Debug.Log ("Component Slot Added");
					layout.ComponentSlots.Add(Instantiate(SelectedComponent, hit.transform.position+offset, SelectedComponent.transform.rotation) as GameObject);
				}
			}
			return true;
		}
		else if(hit.transform.gameObject.name == "ComponentSlot(Clone)" )
		{
			if(Input.GetKey(Remove))
			{
				if(FinishedRemoving){
					layout.ComponentSlots.Remove(hit.transform.gameObject);
					Destroy(hit.transform.gameObject);
					Debug.Log("ComponentSlot Removed");
					FinishedRemoving = false;
				}

			}else if(SelectedComponentID != ComponentSlotID)
			{
				hit.transform.gameObject.GetComponent<ColorState>().Selected = true;
				layout.ShipComponents.Add(Instantiate(SelectedComponent, hit.transform.position+offset*2, SelectedComponent.transform.rotation) as GameObject);
				Debug.Log("Component "+SelectedComponent.name+" Added.");
			}
			return true;
		}
		else if(hit.transform.gameObject.tag == "Component")
		{
			if(Input.GetKey(Remove) && FinishedRemoving)
			{
				Debug.Log("Component "+hit.transform.gameObject.name+" removed.");
				layout.ShipComponents.Remove(hit.transform.gameObject);
				Destroy(hit.transform.gameObject);
				Debug.Log("Component slot occupied.");
				FinishedRemoving = false;
			}
			return true;
		}
		return false;
	}
	void CheckButtons()
	{
		SelectedComponent = hit.transform.gameObject.GetComponent<Button_ShipComponent>()._ShipComponent;
		SelectedComponentID = SelectedComponent.GetComponent<ShipComponent>().ComponentID;
		Debug.Log("Button "+SelectedComponent.name+" selected");
	}
}
