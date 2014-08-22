using UnityEngine;
using System.Collections;

public class PlayerShip : TurnBasedUnit 
{

    public override IEnumerator ExecuteTurn()
    {
        Debug.Log(unitName + " (PlayerShip) starts turn");
        projector.enabled = true;

        yield return base.ExecuteTurn();

        while(!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        projector.enabled = false;
        Debug.Log(unitName + " (PlayerShip) ends turn");
    }
}
