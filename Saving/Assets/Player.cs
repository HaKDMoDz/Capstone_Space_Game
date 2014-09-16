using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    int health;
    List<Vector3> positions;

	void Start()
    {
        //Debug.Log(Application.persistentDataPath);
        PersistentData.Instance.Load();
        health = PersistentData.Instance.Health;
        positions = PersistentData.Instance.Positions;
    }
	void Update () {
	    if(Input.GetKeyDown(KeyCode.W))
        {
            health += 10;
            PersistentData.Instance.Health = health;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            health -= 10;
            PersistentData.Instance.Health = health;
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            positions = PersistentData.Instance.GenerateNewPositions();
            PersistentData.Instance.Positions = positions;

        }
        
        if(Input.GetKeyDown(KeyCode.F5))
        {
            PersistentData.Instance.Save();
        }

	}
    
    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 100, 50), "Health: " + health);
        for (int i = 0; i < positions.Count; i++)
        {
            GUI.Label(new Rect(Screen.width / 2 - (positions.Count / 2 * 100) + i * 100, Screen.height / 2 + 50, 100, 50), positions[i].ToString());
        }
        
    }
}
