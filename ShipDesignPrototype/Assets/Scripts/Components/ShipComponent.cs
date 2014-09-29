using UnityEngine;
using System.Collections;

public class ShipComponent : MonoBehaviour 
{
    public int ID;
    public string componentName;
    public bool unlocked;

    public enum ComponentType { Weapon, Defense, Power, Support}
    [SerializeField]
    protected ComponentType compType;

    public ComponentType CompType
    {
        get { return compType; }
        //set { compType = value; }
    }


    bool selected;
    public bool Selected
    {
        get { return selected; }
        set 
        {
            selected = value;
            selectProjector.enabled = value;
        }
    }

    //cached vars
    Projector selectProjector;

    public virtual void Init()
    {
        selectProjector = GetComponentInChildren<Projector>();
    }

    public virtual void Activate(System.Action onComplete)
    {

    }

}
