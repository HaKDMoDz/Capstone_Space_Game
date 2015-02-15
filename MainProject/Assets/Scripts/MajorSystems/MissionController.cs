using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MissionController : Singleton<MissionController>
{
    private Action[] acceptMissionFunctions = new Action[10];
    private Action[] completeMissionFunctions = new Action[10];

    public void Awake()
    {
        acceptMissionFunctions[0] = null;

    }

    public void AddMission(int _index, Action F)
    {
        acceptMissionFunctions[_index] = F;
    }
    public void AddMissionComplete(int _index, Action F)
    {
        completeMissionFunctions[_index] = F;
    }

    public void AcceptMission(int _index)
    {
        Action accpetMission = acceptMissionFunctions[_index];
        accpetMission();
    }

    public void CompleteMission(int _index)
    {
        Action completeMission = completeMissionFunctions[_index];
        completeMission();
    }
}
