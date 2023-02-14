using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;

public class EveningSceneManager : MonoBehaviour
{
    public GameObject rentalFeePopUp;
    public Text rentalFeeText;

    public GameObject receiptPopUp;
    public Text gainText;
    public Text reputationText;
    public Text successText;
    public Text failText;

    public GameObject buyPopUp;
    public Text ingredientName;
    private int ingredientPrice;
    public Text ingredientPriceText;
    private int ingredientIndex;
    public GameObject ingredientPurchaseDone;
    public GameObject ingredientBuyBtn;
    public Text recipeName;
    public Text recipePrice;
    public GameObject recipePurchaseDone;
    public GameObject recipeBuyBtn;
    public Text machineName;
    public Text machinePrice;
    public GameObject machinePurchaseDone;
    public GameObject machineBuyBtn;

    [SerializeField] private JsonManager jsonManager;
    private GameDataUnit gameDataUnit;
    private Ingredient ingredientData;

    void Start()
    {   
        //Invoke("ShowReceiptPopUp", 1f);
        ShowReceiptPopUp();
    }
// ------------------- 영수증 (하루 마감) -----------------------

    public void ShowReceiptPopUp(){
        receiptPopUp.SetActive(true);
        gainText.text = $"이익 : {DaySceneManager.todayCoin}";
        reputationText.text = $"명성 : {DaySceneManager.todayReputation}";
        successText.text = $"성공 : {DaySceneManager.successCount}";
        failText.text = $"실패 : {DaySceneManager.failCount}";
    }

    public void ExitReceiptPopUp(){
        receiptPopUp.SetActive(false);

        if(GameManager.instance.userData.day%3 == 0){
            Invoke("ShowRentalFeePopUp", 1f);
        }
        else{
            Invoke("ShowBuyPopUp", 1f);
        }
   }
// -------------------------- 월세 ---------------------------

    public void ShowRentalFeePopUp(){
        if(GameManager.instance.userData.day%3 == 0){
            rentalFeeText.text = "이번엔 " + SetRentalFee().ToString() + " G 야!"; 
            rentalFeePopUp.SetActive(true);
        }
    }

    public void ExitRentalFeePopUp(){
        GameManager.instance.userData.gold -= SetRentalFee();
        rentalFeePopUp.SetActive(false);

        Invoke("ShowBuyPopUp", 1f);
    }

    //명성에 따라 월세 지정
    private int SetRentalFee(){ 
        int rentalFee=0;
        if(GameManager.instance.userData.reputation<=30){ // 명성 <= 30
            rentalFee = 1500;
        }
        else if(GameManager.instance.userData.reputation<=60){ // 30 < 명성 <= 60
            rentalFee = 2000;
        } 
        else if(GameManager.instance.userData.reputation<=90){ // 60 < 명성 <= 90
            rentalFee = 2500;
        }
        else{ // 90 < 명성 <= 100
            rentalFee = 3000;
        }

        return rentalFee;
    }
//------------------------- 재료, 레시피, 기계 구매 --------------------

    public void ShowBuyPopUp(){
        buyPopUp.SetActive(true);
        ingredientPurchaseDone.SetActive(false);
        recipePurchaseDone.SetActive(false);
        machinePurchaseDone.SetActive(false);

        //여기작성해야함
        ingredientIndex = ChooseIngredient();
        gameDataUnit = jsonManager.LoadJson<GameDataUnit>("Ingredient");
        ingredientData = gameDataUnit.ingredientArray[ingredientIndex];
        ingredientName.text = ingredientData.nameKor.ToString();
        ingredientPrice = ingredientData.price;
        ingredientPriceText.text = ingredientPrice.ToString() + " G";
        
    }

    // 재료 뽑기
    private int ChooseIngredient(){
        bool blenderUnlock = GameManager.instance.userData.machineUnlock[4];
        bool swingBottleUnlock = GameManager.instance.userData.machineUnlock[5];

        List<Tuple<int, float>> probList = new List<Tuple<int, float>>();
        gameDataUnit = jsonManager.LoadJson<GameDataUnit>("Ingredient");

        if(blenderUnlock && swingBottleUnlock){ // 블렌더, 스윙보틀 모두 해금
            for(int i=0; i<gameDataUnit.ingredientArray.Length; i++){
                ingredientData = gameDataUnit.ingredientArray[i];
                if(ingredientData.bOsO != 0.0f){
                    probList.Add(new Tuple<int, float>(ingredientData.index, (float)ingredientData.bOsO));
                }
            }
        }
        else if(blenderUnlock && swingBottleUnlock==false){ // 블렌더만 해금
            for(int i=0; i<gameDataUnit.ingredientArray.Length; i++){
                ingredientData = gameDataUnit.ingredientArray[i];
                if(ingredientData.bOsX != 0){
                    probList.Add(new Tuple<int, float>(ingredientData.index, ingredientData.bOsX));
                }
            }
        }
        else if(blenderUnlock==false && swingBottleUnlock){ // 스윙보틀만 해금
            for(int i=0; i<gameDataUnit.ingredientArray.Length; i++){
                ingredientData = gameDataUnit.ingredientArray[i];
                if(ingredientData.bXsO != 0){
                    probList.Add(new Tuple<int, float>(ingredientData.index, ingredientData.bXsO));
                }
            } 
        }
        else{ // 블렌더, 스윙보틀 둘다 해금 X
            for(int i=0; i<gameDataUnit.ingredientArray.Length; i++){
                ingredientData = gameDataUnit.ingredientArray[i];
                if(ingredientData.bXsX != 0){
                    probList.Add(new Tuple<int, float>(ingredientData.index, ingredientData.bXsX));
                }
            }
        }

        //확률 오름차순 정렬
        probList.Sort((a, b) => a.Item2.CompareTo(b.Item2));

        while(true){
            float randomValue = UnityEngine.Random.value;
            float criteria = 0.0f;
            for(int i=0; i<probList.Count; i++){
                criteria += probList[i].Item2;
                if(randomValue <= criteria && !GameManager.instance.userData.ingredientUnlock[probList[i].Item1]){
                    return probList[i].Item1;
                }
            }
        
        Debug.Log("error");
        return -1;
        }    
    }

    // 재료 사기 버튼
    public void BuyIngredientBtn(){
        if(GameManager.instance.userData.gold >= ingredientPrice){
            ingredientPurchaseDone.SetActive(true);
            ingredientBuyBtn.SetActive(false);
            Debug.Log($"ingredientIndex = {ingredientIndex}");
            GameManager.instance.userData.ingredientUnlock[ingredientIndex]=true;
            GameManager.instance.userData.gold -= ingredientPrice;
            jsonManager.SaveData(GameManager.instance.userData);
        }
        else{
            Debug.Log("구매불가");
        }
    }

    public void AnotherBtn(){
        if(GameManager.instance.userData.gold>=100){
            GameManager.instance.userData.gold -= 100;
            ShowBuyPopUp();
        }
        else{
            Debug.Log("돈 부족");
        }
    }

    public void ExitBuyPopUp(){
        SceneManager.LoadScene("NightScene");
    }
}
