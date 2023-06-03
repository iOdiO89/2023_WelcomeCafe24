using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TutorialScriptWrapper
{
    public TutorialScript[] tutorialScriptArray;

    public void Parse()
    {
        for(int i =0; i< tutorialScriptArray.Length; i++)
        {
            Debug.Log("index: " + tutorialScriptArray[i].tutorialIndex 
                + " / EventName: " + tutorialScriptArray[i].tutorialEventName
                + " / Script: " + tutorialScriptArray[i].tutorialScript);
            tutorialScriptArray[i].Parse();
        }
    }
}
