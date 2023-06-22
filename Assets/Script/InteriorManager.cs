using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteriorManager : MonoBehaviour
{   
    [SerializeField] Image[] machines;

    public void SetInterior(){
        string path1 = "Images/";
        string path2 = "Images/";
        if(GameManager.instance.daySceneActive){ // 낮일 때
            path1 += "Day/Machine/";
            path2 += "Day/Machine/";
        }
        else{   // 밤일 때
            path1 += "Night/Machine/";
            path2 += "Night/Machine/";
        }

        switch(GameManager.instance.userData.machineLevel){
                case 1:
                    path1+="level1";
                    Debug.Log("Machine Level 1");
                    break;
                case 2:
                    path1+="level2";
                    path2+="level1";
                    Debug.Log("Machine Level 2");
                    break;
                case 3:
                    path1+="level3";
                    path2+="level3";
                    Debug.Log("Machine Level 3");
                    break;
                default:
                    Debug.Log("machine image loading error");
                    break;
            }
        Sprite[] imageArray1 = Resources.LoadAll<Sprite>(path1);
        Sprite[] imageArray2 = Resources.LoadAll<Sprite>(path2);

        if(GameManager.instance.userData.machineLevel != 1){
            for(int i=0, j=0; i<machines.Length; i++, j++){
                if(GameManager.instance.userData.machineUnlock[j]){
                    machines[i].sprite = imageArray1[i];
                }
                else{
                    machines[i].sprite = imageArray2[i];
                }

                if(j==1) j++;
            }
        }
        else{ // level1 일 때
            for(int i=0, j=0; i<machines.Length; i++, j++){
                if(GameManager.instance.userData.machineUnlock[j]){
                    machines[i].sprite = imageArray1[i];
                }

                if(j==1) j++;
            }
        }

    }
}