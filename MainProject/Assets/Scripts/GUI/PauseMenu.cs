/*
  PauseMenu.cs
  Mission: Invasion
  Created by Rohun Banerji on March 28, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour 
{

    public void ReturnToMainMenu()
    {
        GameController.Instance.ChangeScene(GameScene.MainMenu);
    }
    public void Quit()
    {
        Application.Quit();
    }

}
