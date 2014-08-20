using UnityEngine;
using System.Collections;

public class TurnBasedEntity : MonoBehaviour 
{
    public float turnDelay;
    public string name;
    
    private float timeLeftToTurn;

    public float TimeLeftToTurn
    {
        get { return timeLeftToTurn; }
        set 
        {
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

    TextMesh textMesh;

    public virtual void Awake()
    {
        TimeLeftToTurn = turnDelay;
        textMesh = GetComponentInChildren<TextMesh>();
    }

    public virtual IEnumerator StartTurn()
    {
        yield return null;
    }

    void Update()
    {
        textMesh.text = name + " " + timeLeftToTurn;

    }

}
