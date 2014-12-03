using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameSaveSystem 
{
    public void Save(GameData gameData)
    {
        #if FULL_DEBUG
        Debug.Log("Saving game data");
        #endif
    }
    public bool Load(out GameData gameData)
    {
        #if FULL_DEBUG
        Debug.Log("Loading game data");
        #endif

        gameData = null;
        return false;
    }

	
}

