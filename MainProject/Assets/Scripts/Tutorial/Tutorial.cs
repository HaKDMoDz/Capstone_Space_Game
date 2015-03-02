using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Tutorial : Singleton<Tutorial> 
{
    public enum TutorialType 
    { 
        TurnOrder,
        //movement tutorials
        MovementHowTo, MovementUI, MovementPowerCost, MoveCostThruster,
        //targeting an enemy
        ClickEnemyToEngage, 
        //component panel
        ShowsComponents, Hotkeys, PowerDrainOnSelect,
        //enemy target panel
        TargetedEnemyShip, ClickOnCompToFire,ShieldHP,
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
        tutorialType_entry_table[type].panel.gameObject.SetActive(show);
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
            type_entry.Value.panel.AddOnClickListener(() => ShowNextTutorial(currentType));
        }
        StartTutorial();
    }

    [Serializable]
    public class TutorialEntry
    {
        public TutorialType type;
        public TutorialPanel panel;
        public bool show;

    }
}
