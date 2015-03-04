using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class TutorialSystem : Singleton<TutorialSystem> 
{
    public enum TutorialType 
    { 
        TurnOrder,
        //movement tutorials
        MovementHowTo, MovementUI, MovementPowerCost, MoveCostThruster,
        //targeting an enemy
        ClickEnemyToEngage, 
        //component panel
        ComponentPanel, ComponentSelection, Hotkeys, ActivationCost,  
        //enemy target panel
        TargetedEnemyShip, ClickOnCompToFire,EnemyShieldHP,
        //End turn
        EndTurn
    }

    [SerializeField]
    private List<TutorialType> tutorialSequence;
    
    [SerializeField]
    private List<TutorialEntry> tutorialEntryList;
    
    private Dictionary<TutorialType, TutorialEntry> tutorialType_entry_table;

    public void ShowAllTutorials(bool show)
    {
        GameController.Instance.GameData.tutorialData.ShowTutorials = show;
        foreach (GameObject panel in tutorialType_entry_table.Values.Select(tut=>tut.panel.gameObject))
        {
            panel.SetActive(show);
        }
    }
    public void ShowNextTutorial(TutorialType currentType)
    {
        int next = tutorialSequence.IndexOf(currentType);
        next++;

        ShowTutorial(currentType, false);
        if (next < tutorialSequence.Count)
        {
            ShowTutorial(tutorialSequence[next], true);
        }
    }

    public void StartTutorial()
    {
        ShowTutorial(tutorialSequence[0],true);
    }

    public void ShowTutorial(TutorialType type, bool show)
    {
        if(!show)
        {
            tutorialType_entry_table[type].panel.gameObject.SetActive(false);
            return;
        }
        if (!tutorialType_entry_table[type].shown &&
            GameController.Instance.GameData.tutorialData.ShowTutorials)
        {
            tutorialType_entry_table[type].panel.gameObject.SetActive(true);
            tutorialType_entry_table[type].shown = true;    
        }
    }

    private void Awake()
    {
        tutorialType_entry_table = tutorialEntryList.ToDictionary(entry=>entry.type, entry=>entry);
    }
    private void Start()
    {
        foreach (var type_entry in tutorialType_entry_table)
        {
            //Debug.Log("Type " + type_entry.Key + " toggle " + type_entry.Value.show);
            TutorialType currentType = type_entry.Key;
            TutorialPanel panel = type_entry.Value.panel;
            panel.AddOnClickListener(() => 
                {
                    if (!panel.ToggleIsOn)
                    {
                        ShowAllTutorials(false);
                    }
                    else 
                    {
                        if (panel.AutoAdvance)
                        {
                            ShowNextTutorial(currentType);
                        }
                        else
                        {
                            ShowTutorial(currentType, false);
                        }
                    }
                });
        }
        if (GameController.Instance.GameData.tutorialData.ShowTutorials)
        {
            StartTutorial();
        }
    }

    [Serializable]
    public class TutorialEntry
    {
        public TutorialType type;
        public TutorialPanel panel;
        public bool shown;

    }
}
