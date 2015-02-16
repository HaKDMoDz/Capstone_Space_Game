using UnityEngine;
using System.Collections;

public class MissionSelector : MonoBehaviour 
{
    [SerializeField]
    private Transform currentDestination;
    public UnityEngine.Transform CurrentDestination
    {
        get { return currentDestination; }
        set { 
            currentDestination = value;
            StartCoroutine(TrackTarget());
        }
    }


	void Awake () 
    {
        if (currentDestination)
        {
            StartCoroutine(TrackTarget());
        }
	}
	
	IEnumerator TrackTarget()
    {
        transform.LookAt(currentDestination);


        yield return new WaitForFixedUpdate();
        yield return StartCoroutine(TrackTarget());
    }


}
