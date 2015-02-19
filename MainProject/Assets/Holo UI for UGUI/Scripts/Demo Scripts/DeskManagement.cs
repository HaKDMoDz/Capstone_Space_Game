using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DeskManagement : MonoBehaviour {

	public void ToggleDesk1(bool state)
	{
		if (state)
		{
			UIWindow.GetWindowByCustomID(1).Show();
			UIWindow.GetWindowByCustomID(2).Show();
			UIWindow.GetWindowByCustomID(3).Show();
		}
		else
		{
			UIWindow.GetWindowByCustomID(1).Hide();
			UIWindow.GetWindowByCustomID(2).Hide();
			UIWindow.GetWindowByCustomID(3).Hide();
		}
	}
	
	public void ToggleDesk2(bool state)
	{
		if (state)
		{
			UIWindow.GetWindowByCustomID(4).Show();
		}
		else
		{
			UIWindow.GetWindowByCustomID(4).Hide();
		}
	}
	
	public void ToggleDesk3(bool state)
	{
		if (state)
		{
			UIWindow.GetWindowByCustomID(5).Show();
		}
		else
		{
			UIWindow.GetWindowByCustomID(5).Hide();
		}
	}
	
	public void ToggleDesk4(bool state)
	{
		if (state)
		{
			UIWindow.GetWindowByCustomID(6).Show();
		}
		else
		{
			UIWindow.GetWindowByCustomID(6).Hide();
		}
	}
}
