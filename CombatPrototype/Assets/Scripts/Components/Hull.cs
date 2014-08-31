using UnityEngine;
using System.Collections;

public class Hull : MonoBehaviour 
{
    [SerializeField] GameObject structure;
    [SerializeField]
    private float hullHP;

    public float HullHP
    {
        get { return hullHP; }
    }


}
