using UnityEngine;
using System.Collections;

public class TurnBasedUnit : MonoBehaviour 
{

    public float turnDelay;
    public string unitName;

    private float timeLeftToTurn;

    public float TimeLeftToTurn
    {
        get { return timeLeftToTurn; }
        set 
        {
            //sets back to max once turnDelay hits 0
            if (value <= 0.0f)
            {
                timeLeftToTurn = turnDelay;
            }
            else
            {
                timeLeftToTurn = value;
            }
        }
    }

    protected Projector projector;

    public virtual void Awake()
    {
        TimeLeftToTurn = turnDelay;
        projector = GetComponentInChildren<Projector>();

    }
    public virtual IEnumerator ExecuteTurn()
    {
        yield return null;
    }
    
}
