using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnManager : MonoBehaviour
{
    GameObject continuePopUp;

    void Start(){
        //continuePopUp = GameObject.FindWithTag("ContinuePopUp");
        //continuePopUp.SetActive(false);
    }

    public void StartSceneStartNewBtn(){    // 새로하기

    }

    public void StartSceneContinueBtn(){    // 이어하기
        continuePopUp = GameObject.Find("ContinuePopUp");
        continuePopUp.SetActive(false);
        //Debug.Log("setactive success");
    }


}
