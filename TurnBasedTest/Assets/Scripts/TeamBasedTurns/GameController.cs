using UnityEngine;
using System.Collections;

public class GameController : SingletonComponent<GameController> 
{

    void Start()
    {
        StartCoroutine(PlayGame());
    }

    IEnumerator PlayGame()
    {
        while(true)
        {
            yield return StartCoroutine(PlayerTurn());
            yield return StartCoroutine(AITurn());
        }
    }
    
    IEnumerator PlayerTurn()
    {
        Debug.Log("player turn");
        yield return StartCoroutine(PlayerTurnController.Instance.ExecutePlayerTurn());
    }

    IEnumerator AITurn()
    {
        Debug.Log("AI turn");
        yield return StartCoroutine(AITurnController.Instance.ExecuteAITurn());
    }

}
