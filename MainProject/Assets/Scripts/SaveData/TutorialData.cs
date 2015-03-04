using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TutorialData
{
    public bool ShowTutorials;

    public TutorialData()
    {
        ShowTutorials = true;
    }
    public void Serialize(ref SerializedTutorialData sz_TutorialData)
    {
        sz_TutorialData.ShowTutorials = this.ShowTutorials;
    }
}
[Serializable]
public class SerializedTutorialData
{
    public bool ShowTutorials;

    public SerializedTutorialData()
    {
        ShowTutorials = true;
    }
    public void DeSerialize(ref TutorialData tutorialData)
    {
        tutorialData.ShowTutorials = this.ShowTutorials;
    }
}
