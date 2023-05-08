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
                Debug.Log($"{i}번째 machine 이미지 교체");
                machines[i].sprite = imageArray[i];

            }
            if(j==1) j++;
        }
        // string path = "Images/Day/Machine/level";
        // path.append(GameManager.instance.userData.machineLevel.ToString());
        // Image[] tempArr = Resources.LoadAll<Image>(path);
        // for(int i=0; i<6; i++){     
        //     if(GameManager.instance.userData.machineUnlock){

        //     }
        //     else{
                
        //     }
        // }
    }
}

// 디폴트로 들어가는 기계 말고 나머지들은 일단 투명 png 처리하고
// 씬 스타트할 때 마다 확인해서 이미지 바꾸기
// 스크립트는 같이 쓰고 dayScene인지 nightScene인지만 확인해서 하면 될듯??