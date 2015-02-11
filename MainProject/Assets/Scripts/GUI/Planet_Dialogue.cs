using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Planet_Dialogue : MonoBehaviour
{
    [SerializeField]
    private String[] dialogueText;
    public String[] DialogueText
    {
        get { return dialogueText; }
        set { dialogueText = value; }
    }
    
    
    bool panelOpen = false;

    [SerializeField]
    private int id;
    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    private int currentIndex = 0;

    void Awake()
    {
        
    }

    public void OpenPanel()
    {
        panelOpen = !panelOpen;

        transform.FindChild("PlanetUI").FindChild("DialoguePanel").gameObject.SetActive(panelOpen);
        Debug.Log(transform.FindChild("PlanetUI").FindChild("DialoguePanel").FindChild("Text"));
        transform.FindChild("PlanetUI").FindChild("DialoguePanel").FindChild("Text").GetComponent<Text>().text = dialogueText[currentIndex];
    }

    public void ClosePanel()
    {
        currentIndex++;

        if (currentIndex < dialogueText.Length)
        {
            currentIndex = currentIndex % dialogueText.Length;
            transform.FindChild("PlanetUI").FindChild("DialoguePanel").FindChild("Text").GetComponent<Text>().text = dialogueText[currentIndex];
        }
        else
        {
            panelOpen = !panelOpen;
            transform.FindChild("PlanetUI").FindChild("DialoguePanel").gameObject.SetActive(panelOpen);
        }
    }
}
