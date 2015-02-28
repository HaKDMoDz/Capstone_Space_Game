using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SystemManager : MonoBehaviour 
{
    public List<GameObject> Systems;
    public GameObject closestSystem;
    public GameObject Mothership;

    private float closestSystemDistance = 10000.0f;

	void Start () 
    {
        StartCoroutine(CheckForClosestSystem());
	}
	
    public IEnumerator CheckForClosestSystem()
    {

        if (closestSystem)
        {
            closestSystemDistance = (closestSystem.transform.position - Mothership.transform.position).magnitude;
        }
        
        
        foreach (GameObject system in Systems)
        {
            float _distanceToMothership = (system.transform.position - Mothership.transform.position).magnitude;
            if (_distanceToMothership < closestSystemDistance)
            {
                closestSystem = system;
                closestSystemDistance = _distanceToMothership;
               
            }
        }

        Debug.LogWarning("Closest System is: " + closestSystem.name + " at: " + closestSystemDistance);

        yield return new WaitForSeconds(0.016f);
        yield return StartCoroutine(CheckForClosestSystem());
    }
}
