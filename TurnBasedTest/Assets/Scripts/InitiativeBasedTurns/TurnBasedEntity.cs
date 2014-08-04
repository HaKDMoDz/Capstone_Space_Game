using UnityEngine;
using System.Collections;

public class TurnBasedEntity : MonoBehaviour 
{
    public int initiative;

    public virtual IEnumerator StartTurn()
    {
        yield return null;
    }

}
