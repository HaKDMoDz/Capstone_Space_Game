using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnBasedCombatSystem : MonoBehaviour 
{

    List<TurnBasedEntity> entities;

    public bool combatPhaseOn = true;

    float currentTime = 0f;

    List<TurnBasedEntity> turnHistory;

    
    void Start()
    {
        List<TurnBasedEntity> players = FindObjectsOfType<Player>().ToList<TurnBasedEntity>();
        List<TurnBasedEntity> aiCubes = FindObjectsOfType<AICube>().ToList<TurnBasedEntity>();

        entities = new List<TurnBasedEntity>();

        entities.AddRange(players);
        entities.AddRange(aiCubes);

        turnHistory = new List<TurnBasedEntity>();

        StartCoroutine(StartCombat());

    }

    IEnumerator StartCombat()
    {
        int entityCount = entities.Count;
        while (combatPhaseOn && entityCount > 0)
        {
            //sorting according to time left to turn - first in the list has the lowest time remaining until it's turn
            entities = entities.OrderBy(entity => entity.TimeLeftToTurn).ToList<TurnBasedEntity>();
            
            //currentTime gets set to the lowest Time To Turn
            currentTime=entities[0].TimeLeftToTurn;
            //the first unit in the list takes it's turn
            yield return StartCoroutine(entities[0].StartTurn());
            turnHistory.Add(entities[0]);

            
            while(!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
                       

            for (int i = 0; i < entityCount; i++)
            {
                entities[i].TimeLeftToTurn -= currentTime;
            }

            entityCount = entities.Count;
        }
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20, 20, 200, 100));
        GUILayout.BeginHorizontal();
        GUILayout.Label("<size=24>Turn Order: </size>");
        for (int i = 0; i < entities.Count; i++)
        {
            GUILayout.Label( "<size=24> " + entities[i].name + "</size>");
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(Screen.width - 300, 50, 200, Screen.height - 50));
        GUILayout.BeginVertical();
        GUILayout.Label("Turn History");
        for (int i = 0; i < turnHistory.Count; i++)
        {
            GUILayout.Label( "<size=24> " + turnHistory[i].name + "</size>");
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

}
