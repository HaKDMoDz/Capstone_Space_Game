using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AITurnController : SingletonComponent<AITurnController> 
{
     List<AICube> enemies;

    void Start()
     {
         enemies = GameObject.FindObjectsOfType<AICube>().ToList<AICube>();
     }
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

