using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Player : MonoBehaviour {

    int health;
    List<Vector3> positions;
    Dictionary<int, string> table;
    int counter;
    public enum State {Attacking, Fleeing, Seeking, Idle }
    State currentState;

	void Start()
    {
        //Debug.Log(Application.persistentDataPath);
        PersistentData.Instance.Load();
        health = PersistentData.Instance.Health;
        positions = PersistentData.Instance.Positions;
        table = PersistentData.Instance.Table;
        currentState = PersistentData.Instance.CurrentState;

        counter = 2;
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

        if(Input.GetKeyDown(KeyCode.F))
        {
            table.Add(counter, "Two");
            counter++;
            PersistentData.Instance.Table = table;
        }

        if(Input.GetKeyDown(KeyCode.N))
        {
            switch (currentState)
            {
                case State.Attacking:
                    currentState = State.Fleeing;
                    break;
                case State.Fleeing:
                    currentState = State.Seeking;
                    break;
                case State.Seeking:
                    currentState = State.Idle;
                    break;
                case State.Idle:
                    currentState = State.Attacking;
                    break;
                default:
                    break;
            }
            PersistentData.Instance.CurrentState = currentState;
        }

	}
    
    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 100, 50), "Health: " + health);
        for (int i = 0; i < positions.Count; i++)
        {
            GUI.Label(new Rect(Screen.width / 2 - (positions.Count / 2 * 100) + i * 100, Screen.height / 2 + 50, 100, 50), positions[i].ToString());
        }
        for (int i = 0; i < table.Count; i++)
        {
            GUI.Label(new Rect(100, 100 + 50 * i, 100, 50), table.ElementAt(i).Key + ": " + table.ElementAt(i).Value);
        }
        GUI.Label(new Rect(200, 100, 100, 50), currentState.ToString());
    }
}
