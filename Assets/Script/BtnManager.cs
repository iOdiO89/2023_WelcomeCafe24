using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BtnManager : MonoBehaviour
{   
    public List<GameObject> recipeList = new List<GameObject>();
    public List<GameObject> menuList = new List<GameObject>();

    List<bool> activeMenuList = new List<bool>();

    public void StartNewBtn(){    // StartScene-새로하기
        SceneManager.LoadScene("PrologScene");
    }

    public void ExitBtn(){    // StartScene-종료하기 & MainScene-PauseBtn-게임종료
        Application.Quit();
    }

    public void SkipBtn(){  // PrologScene-건너뛰기
        SceneManager.LoadScene("MainScene");
    }

    public void ShowRecipe(int index){
        int recipeSize = recipeList.Count;
        for(int i=0; i<recipeSize; i++){
            if(i==index){
                recipeList[i].SetActive(true);
            }
            else{
                recipeList[i].SetActive(false);
            }
        }
    }

    public void MenuBtn(){
        /*activeMenuList를 활용하여 해당 메뉴가 해금되지 않은 경우
        NotActiveRecipe가 출력되지 않게 해야 함(아직 구현X)*/

        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        string name = clickObject.name;
        string stringRes = name.Substring(name.Length-2);
        int intRes=-1;
        if(!int.TryParse(stringRes, out intRes)){ // 두자리수일때
            stringRes = name.Substring(name.Length-1);
        }
        // print(System.Convert.ToInt32(stringRes) + "번 메뉴");
        ShowRecipe(System.Convert.ToInt32(stringRes)-1);
    }
}