using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TutorialEvent
{
    None, Money, MoneyEnd, Recipe, RecipeEnd, 
    Shelf, ShelfEnd, Cup, CupEnd, Night, NightEnd, TutorialEnd
}

[System.Serializable]
public class TutorialScript
{
    public int tutorialIndex;
    public string tutorialEventName;
    public TutorialEvent tutorialEvent;
    public string tutorialScript;

    public TutorialScript()
    {
        tutorialIndex = 0;
        tutorialEventName = "";
        tutorialEvent = TutorialEvent.None;
        tutorialScript = "";
    }

    public void Parse()
    {
        if (tutorialEventName != "")
        {
            tutorialEvent = (TutorialEvent)Enum.Parse(typeof(TutorialEvent), tutorialEventName);
        }
    }
}
