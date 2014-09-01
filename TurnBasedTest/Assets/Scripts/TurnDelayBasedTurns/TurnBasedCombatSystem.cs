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
        int entityCount = entities.Count; //caching list count

        TurnBasedEntity firstEntity; //will contain the first unit to move every round

        while (combatPhaseOn && entityCount > 0)
        {
            
            //gets the unit with the least TimeLeftToTurn
            firstEntity = entities.Aggregate((current, next) =>
                                        current.TimeLeftToTurn < next.TimeLeftToTurn
                                        ? current : next);


            //records the current unit's time left to turn, to subtract from all units
            currentTime = firstEntity.TimeLeftToTurn;

            //List<TurnBasedEntity> numUnitsWithSameTime = entities.Where(unit => unit.TimeLeftToTurn == currentTime);

            //Debug.Log(numUnitsWithSameTime.Count);
            int numUnitsWithSameTime = entities.Count(unit => unit.TimeLeftToTurn == currentTime);
            if (numUnitsWithSameTime > 1)
            {
                List<TurnBasedEntity> unitsWithSameTime = entities.Where(unit => unit.TimeLeftToTurn == currentTime).ToList(); ;

                for (int i = 0; i < numUnitsWithSameTime; i++)
                {
                    int unitToTakeTurn = Random.Range(0, numUnitsWithSameTime-i);
                    yield return StartCoroutine(unitsWithSameTime[unitToTakeTurn].StartTurn());
                    turnHistory.Add(unitsWithSameTime[unitToTakeTurn] );
                    unitsWithSameTime.RemoveAt(unitToTakeTurn);
                }

            }
            else
            {
                //the first unit takes it's turn
                yield return StartCoroutine(firstEntity.StartTurn());
                turnHistory.Add(firstEntity);

            }
            //adds to the turn history - mainly for GUI purposes

            
            //while(!Input.GetKeyDown(KeyCode.Return))
            //{
            //    yield return null;
            //}
                       
            //subtracts the currentTime from all units' timers
            for (int i = 0; i < entityCount; i++)
            {
                entities[i].TimeLeftToTurn -= currentTime;
            }

            //re-caching list size, in case any unit got removed
            entityCount = entities.Count;
        }
    }

    void OnGUI()
    {
        //GUILayout.BeginArea(new Rect(20, 20, 1000, 500));
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("<size=24>Current Time: "+currentTime+" </size>");
        //GUILayout.Label("<size=24>Turn Order: </size>");
        //for (int i = 0; i < entities.Count; i++)
        //{
        //    GUILayout.Label("<size=24> " + entities[i].name + "</size>");
        //}
        //GUILayout.EndHorizontal();
        //GUILayout.EndArea();

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
