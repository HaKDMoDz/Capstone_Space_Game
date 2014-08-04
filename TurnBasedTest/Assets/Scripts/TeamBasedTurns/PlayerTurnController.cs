using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlayerTurnController : SingletonComponent<PlayerTurnController> 
{
    public List<Player> players;


    public IEnumerator ExecutePlayerTurn()
    {
        for (int i = 0; i < players.Count; i++)
        {

            yield return StartCoroutine(players[i].StartTurn());
            yield return null;
        }
        Debug.Log("Player turn over");
    }

}
