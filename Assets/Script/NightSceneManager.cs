using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightSceneManager : MonoBehaviour
{
    public Text goldText;
    public Text reputationText;


    void Update(){
        SetValues();
    }

    private void SetValues(){
        goldText.text = "G: " + GameManager.instance.userData.gold.ToString();
        reputationText.text = "명성: " + GameManager.instance.userData.reputation.ToString();
    }

}
