using UnityEngine;
using System.Collections;

public class AIShip : TurnBasedUnit 
{
    public override IEnumerator ExecuteTurn()
    {
        Debug.Log(unitName + " (AI Ship) starts turn");
        projector.enabled = true;

        yield return base.ExecuteTurn();

        yield return new  WaitForSeconds(1.0f);

        projector.enabled = false;
        Debug.Log(unitName + " (AI Ship) Ends turn");
    }

}
