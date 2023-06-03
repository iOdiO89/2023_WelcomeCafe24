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
    [HideInInspector]public GameDataUnit gameDataUnit;

    [HideInInspector] public bool daySceneActive;
    [HideInInspector]public bool continueBool;

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

    void Start(){
        userData = new UserDataClass();
        SetGameDataUnit();
    }

    public void ContinueBtn(){  // StartScene-이어하기
        // userData = new UserDataClass();
        userData = jsonManager.LoadData();
        SoundManager.instance.PlayEffect("button");
        if(userData == null){ // 저장된 데이터가 없는데 이어하기 하려는 경우
            Debug.Log("File Not Exists Yet");
        }
        else{
            Debug.Log("File Exists - Continue");
            daySceneActive = false;
            continueBool = true; 
            SceneManager.LoadScene("DayScene");
        }
    }

    public void StartNewBtn(){    // StartScene-새로하기
        // userData = new UserDataClass();
        jsonManager.SaveData<UserDataClass>(userData);
        Debug.Log("Start New");
        daySceneActive = false;
        SoundManager.instance.PlayEffect("button");
        SceneManager.LoadScene("TutorialScene");
    }
    
    public void ExitBtnStartScene(){ // StartScene-종료하기
        SoundManager.instance.PlayEffect("button");
        Application.Quit();
    }

    public void SetGameDataUnit()
    {
        IngredientArray getIngredientArr = jsonManager.LoadJson<IngredientArray>("Ingredient");
        RecipeArray getRecipeArr = jsonManager.LoadJson<RecipeArray>("Recipe");
        MachineArray getMachineArr = jsonManager.LoadJson<MachineArray>("Machine");
        gameDataUnit.ingredientArray = getIngredientArr.ingredientArray;
        gameDataUnit.recipeArray = getRecipeArr.recipeArray;
        gameDataUnit.machineArray = getMachineArr.machineArray;
    }

    /*private void OnApplicationPause(bool pause){
        if(pause){
            jsonManager.SaveData(GameManager.instance.userData);
        }
    }*/

}