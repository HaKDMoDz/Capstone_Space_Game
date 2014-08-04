using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnController : MonoBehaviour 
{
    List<TurnBasedEntity> entities;

    public bool playingGame=true;

    void Start()
    {
        List<TurnBasedEntity> players = FindObjectsOfType<Player>().ToList<TurnBasedEntity>();
        List<TurnBasedEntity> aiCubes = FindObjectsOfType<AICube>().ToList<TurnBasedEntity>();

        entities = new List<TurnBasedEntity>();

        entities.AddRange(players);
        entities.AddRange(aiCubes);

        //sort turn list based on initiative
        entities = entities.OrderBy(entity => entity.initiative).ToList<TurnBasedEntity>();

        StartCoroutine(PlayGame());
    }

    IEnumerator PlayGame()
    {
        while(playingGame)
        {
            foreach (TurnBasedEntity entity in entities)
            {
                yield return StartCoroutine(entity.StartTurn());
            }
        }
    }


}
