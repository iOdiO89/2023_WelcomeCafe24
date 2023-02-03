using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    // 재화, 명성
    public static int gold=0;
    public static int reputation=0;

    public JsonManager jsonManager;
    public UserDataClass userData;

    public static GameManager instance;
    public static GameManager Instance{
        get{
            if(null == instance){
                return null;
            }
            return instance;    
        }
    }

    void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    public void ContinueBtn(){  // StartScene-이어하기
        userData = new UserDataClass();
        userData = jsonManager.LoadData();
        if(userData == null){ // 저장된 데이터가 없는데 이어하기 하려는 경우
            Debug.Log("File Not Exists Yet");
        }
        else{
            Debug.Log("File Exists");
            Debug.Log("Continue");
            SceneManager.LoadScene("MainScene");
        }
    }

    public void StartNewBtn(){    // StartScene-새로하기
        userData = new UserDataClass();
        jsonManager.SaveData<UserDataClass>(userData);
        Debug.Log("Start New");
        SceneManager.LoadScene("PrologScene");
    }
    
    public void ExitBtnStartScene(){ // StartScene-종료하기
        Application.Quit();
    }
    
    public void Temp22Btn(){
        userData = new UserDataClass();
        jsonManager.SaveData(userData);
        Debug.Log(userData.gold);
    }

    /*public void TempBtn(){
        ingredientRoot = jsonManager.LoadJson<GameDataRoot>("ingredient");
        ingData = ingredientRoot.ingredientArray[0];
        Debug.Log(ingData.ingNameEng);
    }*/


    /*private void OnApplicationPause(bool pause){
        if(pause){
            jsonManager.SaveData(GameManager.instance.userData);
        }
    }*/

}