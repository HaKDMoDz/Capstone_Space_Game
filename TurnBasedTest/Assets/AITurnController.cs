using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AITurnController : SingletonComponent<AITurnController> 
{
    public List<AICube> enemies;

    public IEnumerator ExecuteAITurn()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            yield return StartCoroutine(enemies[i].StartTurn());
            yield return null;

        }
        Debug.Log("AI end turn");
    }

}

