using UnityEngine;
using System.Collections;

public class EventSpawner : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        //SystemLog.addMessage("Event Spawner Created...");
        StartCoroutine(MakeSomeNoiseEvent());
	}
	
    IEnumerator MakeSomeNoiseEvent()
    {
        SystemLog.addMessage("tick");
        yield return new WaitForSeconds(30.0f);
        StartCoroutine(MakeSomeNoiseEvent());
    }
	// Update is called once per frame
	void Update () 
    {
	
	}
}
