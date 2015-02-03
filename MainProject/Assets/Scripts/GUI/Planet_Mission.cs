using UnityEngine;
using System.Collections;

public class Planet_Mission : MonoBehaviour 
{

    public SystemObject destination;
    public GameObject[] targets;
    [SerializeField]
    private bool completed;
    public bool Completed
    {
        get { return completed; }
        set { completed = value; }
    }
    [SerializeField]
    private int creditReward;
    public int CreditReward
    {
        get { return creditReward; }
        set { creditReward = value; }
    }

	void Awake () 
    {

	}
	
}
