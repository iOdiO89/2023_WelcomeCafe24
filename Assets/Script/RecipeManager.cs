using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour
{   
    public List<GameObject> recipeList = new List<GameObject>();
    public List<GameObject> menuList = new List<GameObject>();
    public List<Text> menuTextList = new List<Text>();
    public GameObject notActiveRecipe;
    public Text notActiveRecipeText;

    //데이터 관련
    public JsonManager jsonManager;
    private GameDataUnit gameDataUnit;
    private Recipe recipeData;

    private Color transparentColor;

    void Update(){
        SetRecipeColor();
    }
    
    public void SetRecipeColor(){
        transparentColor = menuTextList[0].color;
        transparentColor.a = 0.3f;
        for(int i=0; i<21; i++){
            if(!GameManager.instance.userData.recipeUnlock[i]){
                menuTextList[i].color = transparentColor;
            }
        }
    }

    public void ShowRecipe(int index){
        notActiveRecipe.SetActive(false);

        int recipeSize = recipeList.Count;
        for(int i=0; i<recipeSize; i++){
            if(i==index){
                if(GameManager.instance.userData.recipeUnlock[i]){
                    recipeList[i].SetActive(true);
                }
                else{
                    gameDataUnit = jsonManager.LoadJson<GameDataUnit>("Recipe");
                    recipeData = gameDataUnit.recipeArray[i];
                    notActiveRecipeText.text = recipeData.nameKor + " 제조법";
                    notActiveRecipe.SetActive(true);
                }
            }
            else{
                recipeList[i].SetActive(false);
            }
        }
    }

    public void MenuBtn(){
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        string name = clickObject.name;
        string stringRes = name.Substring(name.Length-2);
        int intRes=-1;
        if(!int.TryParse(stringRes, out intRes)){ // 두자리수일때
            stringRes = name.Substring(name.Length-1);
        }
        print(System.Convert.ToInt32(stringRes) + "번 메뉴");
        ShowRecipe(System.Convert.ToInt32(stringRes));
    }
}