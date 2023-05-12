using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteriorManager : MonoBehaviour
{   
    [SerializeField] Image[] machines;

    void Start(){
        string path = "Images/";
        if(GameManager.instance.daySceneActive){ // 낮일 때
            path += "Day/Machine/";
        }
        else{   // 밤일 때
            path += "Night/Machine/";
        }
        switch(GameManager.instance.userData.machineLevel){
                case 1:
                    path+="level1";
                    Debug.Log("Machine Level 1");
                    break;
                case 2:
                    path+="level2";
                    Debug.Log("Machine Level 2");
                    break;
                case 3:
                    path+="level3";
                    Debug.Log("Machine Level 3");
                    break;
                default:
                    Debug.Log("machine image loading error");
                    break;
            }
        Sprite[] imageArray = Resources.LoadAll<Sprite>(path);

        for(int i=0, j=0; i<machines.Length; i++, j++){
            if(GameManager.instance.userData.machineUnlock[j]){
                machines[i].sprite = imageArray[i];

            }
            if(j==1) j++;
        }
    }
}