/*
  TutorialSystem.cs
  Mission: Invasion
  Created by Rohun Banerji on Mar 2/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class TutorialSystem : Singleton<TutorialSystem> 
{
    public enum TutorialType 
    { 
        //Combat
        //movement tutorials
        MovementHowTo, MovementUI, MovementPowerCost, MoveCostThruster,
        //targeting an enemy
        StartTacticalView, TacticalViewControls, ClickEnemyToEngage, 
        //component panel
        ComponentPanel, ComponentSelection, Hotkeys, ActivationCost,  
        //enemy target panel
        TargetedEnemyShip, ClickOnCompToFire,EnemyShieldHP,
        //End turn
        EndTurn,
        //Ship Design
        //Build
        BuildHull, BuildComponent, DragPaint,
        //stats
        ShipStats,
        //save
        SaveShip, BuildFleet,
        ReturnToGalaxy  
    }

    [SerializeField]
    private List<TutorialType> tutorialSequence;
    [SerializeField]
    private List<TutorialEntry> tutorialEntryList;
    [SerializeField]
    private Toggle tutorialOptionsToggle;
    
    private Dictionary<TutorialType, TutorialEntry> tutorialType_entry_table;

    public void ToggleTutorials()
    {
        if(tutorialOptionsToggle.isOn)
        {
            foreach (var type_entry in tutorialType_entry_table)
            {
                type_entry.Value.shown = false;
                type_entry.Value.panel.Toggle.isOn = false;
            }
            GameController.Instance.GameData.tutorialData.ShowTutorials = true;
            StartTutorial();
        }
        else
        {
            ShowAllTutorials(false);
        }
    }
    public void ShowAllTutorials(bool show)
    {
        GameController.Instance.GameData.tutorialData.ShowTutorials = show;
        foreach (GameObject panel in tutorialType_entry_table.Values.Select(tut=>tut.panel.gameObject))
        {
            panel.SetActive(show);
        }
        tutorialOptionsToggle.isOn = show;
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
            //Debug.Log("Hide Tutorial " + type + " panel: " + tutorialType_entry_table[type].panel.gameObject.name);
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
        //foreach (var item in tutorialType_entry_table)
        //{
        //    Debug.Log("Type " + item.Key + " panel " + item.Value.panel.name);
        //}
    }
    private void Start()
    {
        tutorialOptionsToggle.isOn = GameController.Instance.GameData.tutorialData.ShowTutorials;
        foreach (var type_entry in tutorialType_entry_table)
        {
            //Debug.Log("Type " + type_entry.Key + " toggle " + type_entry.Value.show);
            TutorialType currentType = type_entry.Key;
            TutorialPanel panel = type_entry.Value.panel;
            panel.Toggle.isOn = false;
            if(!panel.AutoAdvance && !panel.TurnOffOnOk)
            {
                panel.OkButton.gameObject.SetActive(false);
                panel.Toggle.gameObject.SetActive(false);
            }
            panel.AddOnClickListener(() => 
                {
                    if (panel.ToggleIsOn)
                    {
                        ShowAllTutorials(false);
                    }
                    else 
                    {
                        if (panel.AutoAdvance)
                        {
                            ShowNextTutorial(currentType);
                        }
                        else if(panel.TurnOffOnOk)
                        {
                            panel.gameObject.SetActive(false);
                        }
                        else
                        {
                            ShowTutorial(currentType, false);
                        }
                    }
                });
        }
        //if (GameController.Instance.GameData.tutorialData.ShowTutorials)
        //{
        //    StartTutorial();
        //}
    }

    [Serializable]
    public class TutorialEntry
    {
        public TutorialType type;
        public TutorialPanel panel;
        public bool shown;

    }
}
