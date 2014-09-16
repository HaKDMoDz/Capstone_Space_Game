using UnityEngine;
using System.Collections;

public class ShipComponent : MonoBehaviour 
{
    public string componentName;

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
