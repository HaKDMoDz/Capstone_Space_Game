using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TestData : ScriptableObject 
{

    public List<int> intList;

    public void AddEntry(int num)
    {
        intList.Add((num));
    }
}
