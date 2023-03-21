using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPaperManager : MonoBehaviour
{
    public GameObject DayBG;
    public GameObject NightBG;

    void Start(){
        DayBG.SetActive(false);
        NightBG.SetActive(false);
        if(GameManager.instance.daySceneActive) DayBG.SetActive(true);
        else NightBG.SetActive(true);
    }
}
