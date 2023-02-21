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
    public Text goldGainText;
    public Text reputGainText;
    public Text successText;
    public Text failText;

    public GameObject buyPopUp;
    public Text ingredientName;
    private int ingredientPrice;
    private int ingredientIndex;
    public Text ingredientPriceText;
    public GameObject ingredientPurchaseDone;
    public GameObject ingredientBuyBtn;

    public Text recipeName;
    public Text recipePriceText;
    private int recipePrice;
    private int recipeIndex;
    public Text recipeGradeText;
    private int recipeGrade;
    public GameObject recipePurchaseDone;
    public GameObject recipeBuyBtn;

    public Text machineName;
    public Text machinePriceText;
    private int machinePrice;
    public Text machineGrade;
    private int machineIndex;
    public GameObject machinePurchaseDone;
    public GameObject machineBuyBtn;

    public Text goldText;

    [SerializeField] private JsonManager jsonManager;
    private Ingredient ingredientData;
    private Recipe recipeData;
    private Machine machineData;

    void Start()
    {   
        Invoke("ShowReceiptPopUp", 0.3f);
        //ShowReceiptPopUp();
    }

    void Update(){
        SetValues();
    }

    private void SetValues(){
        goldText.text = "G: " + GameManager.instance.userData.gold.ToString();
    }
// ------------------- 영수증 (하루 마감) -----------------------

    public void ShowReceiptPopUp(){
        receiptPopUp.SetActive(true);
        goldGainText.text = $"이익 : {DaySceneManager.todayCoin}";
        reputGainText.text = $"명성 : {DaySceneManager.todayReputation}";
        successText.text = $"성공 : {DaySceneManager.successCount}";
        failText.text = $"실패 : {DaySceneManager.failCount}";
    }

    public void ExitReceiptPopUp(){
        receiptPopUp.SetActive(false);

        if(GameManager.instance.userData.day%3 == 0){
            Invoke("ShowRentalFeePopUp", 0.3f);
        }
        else{
            Invoke("ShowBuyPopUp", 0.3f);
            //ShowBuyPopUp();
        }
   }
// -------------------------- 월세 ---------------------------

    public void ShowRentalFeePopUp(){
        if(GameManager.instance.userData.day%3 == 0){
            rentalFeeText.text = "이번엔 " + SetRentalFee().ToString() + " G 야!"; 
            Debug.Log($"rentalFee : {SetRentalFee()}");
            rentalFeePopUp.SetActive(true);
        }
    }

    public void ExitRentalFeePopUp(){
        int rentalFee = SetRentalFee();
        if(GameManager.instance.userData.gold < rentalFee){ // 가지고 있는 돈만 차감
            GameManager.instance.userData.gold = 0;
        }
        else{
            GameManager.instance.userData.gold -= rentalFee;
        }
        rentalFeePopUp.SetActive(false);
        jsonManager.SaveData(GameManager.instance.userData);
        //Invoke("ShowBuyPopUp", 0.3f);
        ShowBuyPopUp();
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
        
        Ingredient();// 재료
        Recipe();// 레시피
        Machine();//기계
       
    }

    private void Ingredient(){
        ingredientIndex = ChooseIngredient();
        if(ingredientBuyBtn.activeSelf){
            ingredientData = GameManager.instance.gameDataUnit.ingredientArray[ingredientIndex];
            ingredientName.text = ingredientData.nameKor; // 이름
            ingredientPrice = ingredientData.price; 
            ingredientPriceText.text = ingredientPrice.ToString() + " G"; //갸격
        }
        //Debug.Log($"{ingredientIndex} : {ingredientData.nameKor}");
    }

    // 재료 뽑기
    private int ChooseIngredient(){
        bool blenderUnlock = GameManager.instance.userData.machineUnlock[4];
        bool swingBottleUnlock = GameManager.instance.userData.machineUnlock[5];

        List<Tuple<int, float>> probList = new List<Tuple<int, float>>();
        //gameDataUnit = jsonManager.LoadJson<GameDataUnit>("Ingredient");

        if(blenderUnlock && swingBottleUnlock){ // 블렌더, 스윙보틀 모두 해금
            for(int i=0; i<GameManager.instance.gameDataUnit.ingredientArray.Length; i++){
                ingredientData = GameManager.instance.gameDataUnit.ingredientArray[i];
                if(ingredientData.bOsO != 0){
                    probList.Add(new Tuple<int, float>(ingredientData.index, (float)ingredientData.bOsO));
                }
            }
        }
        else if(blenderUnlock && swingBottleUnlock==false){ // 블렌더만 해금
            for(int i=0; i<GameManager.instance.gameDataUnit.ingredientArray.Length; i++){
                ingredientData = GameManager.instance.gameDataUnit.ingredientArray[i];
                if(ingredientData.bOsX != 0){
                    probList.Add(new Tuple<int, float>(ingredientData.index, ingredientData.bOsX));
                }
            }
        }
        else if(blenderUnlock==false && swingBottleUnlock){ // 스윙보틀만 해금
            for(int i=0; i<GameManager.instance.gameDataUnit.ingredientArray.Length; i++){
                ingredientData = GameManager.instance.gameDataUnit.ingredientArray[i];
                if(ingredientData.bXsO != 0){
                    probList.Add(new Tuple<int, float>(ingredientData.index, ingredientData.bXsO));
                }
            } 
        }
        else{ // 블렌더, 스윙보틀 둘다 해금 X
            for(int i=0; i<GameManager.instance.gameDataUnit.ingredientArray.Length; i++){
                ingredientData = GameManager.instance.gameDataUnit.ingredientArray[i];
                if(ingredientData.bXsX != 0){
                    probList.Add(new Tuple<int, float>(ingredientData.index, ingredientData.bXsX));
                }
            }
        }
        
        probList.Sort((a, b) => a.Item2.CompareTo(b.Item2)); //확률 오름차순 정렬
        while(true){
            float randomValue = UnityEngine.Random.value;
            float criteria = 0.0f;
            for(int i=0; i<probList.Count; i++){
                criteria += probList[i].Item2;
                if(randomValue <= criteria && !GameManager.instance.userData.ingredientUnlock[probList[i].Item1]){
                    return probList[i].Item1;
                }
            }
        }    
    }

    // 재료 사기 버튼
    public void BuyIngredientBtn(){
        if(GameManager.instance.userData.gold >= ingredientPrice){
            ingredientPurchaseDone.SetActive(true);
            ingredientBuyBtn.SetActive(false);
            //Debug.Log($"ingredientIndex = {ingredientIndex}");
            GameManager.instance.userData.ingredientUnlock[ingredientIndex]=true;
            GameManager.instance.userData.gold -= ingredientPrice;
            Debug.Log($"남은 돈 = {GameManager.instance.userData.gold}");
            jsonManager.SaveData(GameManager.instance.userData);
        }
        else{
            Debug.Log("구매불가");
        }
    }

    private void Recipe(){
        recipeIndex = ChooseRecipe();
        if(recipeBuyBtn.activeSelf){
            recipeData = GameManager.instance.gameDataUnit.recipeArray[recipeIndex];
            recipeName.text = recipeData.nameKor;
            recipeGrade = UnityEngine.Random.Range(1, 4);
            if(recipeGrade==1){
                recipeGradeText.text = "초급";
                recipePrice = recipeData.level1Price; 
                recipePriceText.text = recipePrice.ToString() + " G";
            } 
            else if(recipeGrade==2){
                recipeGradeText.text = "중급";
                recipePrice = recipeData.level2Price; 
                recipePriceText.text = recipePrice.ToString() + " G";
            } 
            else{
                recipeGradeText.text = "고급";
                recipePrice = recipeData.level3Price;
                recipePriceText.text = recipePrice.ToString() + " G";
            } 
        }
        //Debug.Log($"{recipeIndex} : {recipeData.nameKor}");
    }

    // 레시피 뽑기
    private int ChooseRecipe(){
        List<int> recipeList = new List<int>();
        for(int i=0; i<21; i++){
            if(GameManager.instance.userData.recipeUnlock[i]==0) recipeList.Add(i);
        }
        int tempIndex = recipeList[UnityEngine.Random.Range(0, recipeList.Count)];
        return tempIndex;
    }

    public void BuyRecipeBtn(){
        if(GameManager.instance.userData.gold >= recipePrice){
            recipePurchaseDone.SetActive(true);
            recipeBuyBtn.SetActive(false);
            //Debug.Log($"recipeIndex = {recipeIndex}");
            GameManager.instance.userData.recipeUnlock[recipeIndex]=recipeGrade;
            GameManager.instance.userData.gold -= machinePrice;
            Debug.Log($"남은 돈 = {GameManager.instance.userData.gold}");

            jsonManager.SaveData(GameManager.instance.userData);
        }
        else{
            Debug.Log("구매불가");
        }
    }

    private void Machine(){
        machineIndex = ChooseMachine();
        if(machineBuyBtn.activeSelf){
            machineData = GameManager.instance.gameDataUnit.machineArray[machineIndex];
            machineName.text = machineData.nameKor;
            if(GameManager.instance.userData.machineLevel==1){
                machineGrade.text = "보급형";
                machinePrice = machineData.level1Price; 
                machinePriceText.text = machinePrice.ToString() + " G";
            } 
            else if(GameManager.instance.userData.machineLevel==2){
                machineGrade.text = "고급형";
                machinePrice = machineData.level2Price; 
                machinePriceText.text = machinePrice.ToString() + " G";
            } 
            else{
                machineGrade.text = "하이엔드";
                machinePrice = machineData.level3Price;
                machinePriceText.text = machinePrice.ToString() + " G";
            }
        }
 
        //Debug.Log($"{machineIndex} : {machineData.nameKor}");
    }

    // 기계 뽑기
    private int ChooseMachine(){
        switch(GameManager.instance.userData.machineLevel){
            case 1:
                List<Tuple<int, float>> probList = new List<Tuple<int, float>>();
                for(int i=3; i<7; i++){
                    machineData = GameManager.instance.gameDataUnit.machineArray[i];
                    probList.Add(new Tuple<int, float>(machineData.index, machineData.level1Pos));
                }
                probList.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                while(true){
                    float randomValue = UnityEngine.Random.value;
                    float criteria = 0.0f;
                    for(int i=0; i<probList.Count; i++){
                        criteria += probList[i].Item2;
                        if(randomValue <= criteria && !GameManager.instance.userData.machineUnlock[probList[i].Item1]){
                            return probList[i].Item1;
                        }
                    }
                }
            case 2:
            case 3:     
                List<int> machineList = new List<int>();
                for(int i=0; i<7; i++){
                    if(!GameManager.instance.userData.machineUnlock[i]) machineList.Add(i);
                }
                int tempIndex = machineList[UnityEngine.Random.Range(0, machineList.Count)];
                //machineData = GameManager.instance.gameDataUnit.machineArray[tempIndex];
                return tempIndex;
            default:
                Debug.Log("Machine-error");
                return -1;
        }
    }

    // 기계 사기 버튼
    public void BuyMachineBtn(){
        if(GameManager.instance.userData.gold >= machinePrice){
            machinePurchaseDone.SetActive(true);
            machineBuyBtn.SetActive(false);
            //Debug.Log($"machineIndex = {machineIndex}");
            GameManager.instance.userData.machineUnlock[machineIndex]=true;
            GameManager.instance.userData.gold -= machinePrice;
            Debug.Log($"남은 돈 = {GameManager.instance.userData.gold}");

            for(int i=0; i<7; i++){ 
                if(!GameManager.instance.userData.machineUnlock[i]){
                    jsonManager.SaveData(GameManager.instance.userData);
                    return;
                }
            }
            // 현재레벨의 기계가 모두 해금됐으면 다음 레벨로 이동
            if(GameManager.instance.userData.machineLevel!=3) {
                GameManager.instance.userData.machineLevel++;
                for(int i=0; i<7; i++){
                    GameManager.instance.userData.machineUnlock[i]=false;
                }
            }
            jsonManager.SaveData(GameManager.instance.userData);
            return;
        }
        else{
            Debug.Log("구매불가");
        }
    }
// -----------------------목록갱신, 그만사기 버튼-------------------------------------
    public void AnotherBtn(){
        if(GameManager.instance.userData.gold>=100){
            if(ingredientBuyBtn.activeSelf || recipeBuyBtn.activeSelf || machineBuyBtn.activeSelf)
                GameManager.instance.userData.gold -= 100;
            
            int prevIngredientIndex = ingredientIndex;
            int prevRecipeIndex = recipeIndex;
            int prevMachineIndex = machineIndex;
            do{
                Ingredient();// 재료
                Recipe();// 레시피
                Machine();//기계             
            }
            while(prevIngredientIndex==ingredientIndex 
                    || prevRecipeIndex==recipeIndex 
                    || prevMachineIndex==machineIndex);
        }
        else{
            Debug.Log("돈 부족");
        }
    }

    public void ExitBuyPopUp(){
        SceneManager.LoadScene("NightScene");
    }
}
