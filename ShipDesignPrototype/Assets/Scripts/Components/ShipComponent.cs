using UnityEngine;
using System.Collections;

public class ShipComponent : MonoBehaviour 
{
    public uint ID;
    public string componentName;
    public bool unlocked;

    public enum ComponentType { Weapon, Defense, Power, Engine, Support}
    public ComponentType type;


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
