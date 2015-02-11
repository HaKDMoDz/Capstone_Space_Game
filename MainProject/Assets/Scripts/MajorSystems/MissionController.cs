using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MissionController : Singleton<MissionController>
{
    private Action[] acceptMissionFunctions = new Action[10];

    public void Awake()
    {
        acceptMissionFunctions[0] = null;
        //acceptMissionFunctions[1] = (() => mission1());
        //acceptMissionFunctions[2] = (() => mission2());
    }

    public void AddMission(int _index, Action F)
    {
        acceptMissionFunctions[_index] = F;
    }

    public void AcceptMission(int _index)
    {
        Action accpetMission = acceptMissionFunctions[_index];
        accpetMission();
    }

    private void mission2()
    {
        Debug.Log("Mission 2 clicked");
    }
}
