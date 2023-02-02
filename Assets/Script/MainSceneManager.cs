using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using UnityEngine.EventSystems;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField]
    GameObject popupParent;
    [SerializeField]
    GameObject popupBackGround;
    [SerializeField]
    GameObject shelfPopupBoard;
    [SerializeField]
    Button[] itemBtnArray;
    [SerializeField]
    GameObject ingredientPopupBoard;
    [SerializeField]
    Image ingredientImage;
    [SerializeField]
    Field[] fieldsArray;

    public GameObject pausePopUp;
    public Text goldText;
    public Text reputationText;
    

    // 주문 타이머
    public float timeLimit;
    public Text timerText;

    // 주문
    public int orderCount;
    public Text orderText;
    bool orderSuccess;

    [HideInInspector]
    Sprite nowIngredient;

    //데이터 관련
    public JsonManager jsonManager;
    public GameDataUnit gameDataUnit;
    public Recipe recipeData;

    void Update()
    {   
        SetValues();
        if(timeLimit>=0 && pausePopUp.activeSelf==false){
            CountDownTimer();
        }
    }

    public void TempBtn(){
        GameManager.instance.userData.gold += 10;
        GameManager.instance.userData.reputation += 10;
        jsonManager.SaveData<UserDataClass>(GameManager.instance.userData);
    }

    public void SetValues(){
        goldText.text = "G: " + GameManager.instance.userData.gold.ToString();
        reputationText.text = "명성: " + GameManager.instance.userData.reputation.ToString();
    }

    public Text CountDownTimer(bool orderSuccess=false){
        
        timeLimit -= Time.deltaTime; // 1초씩 제거
        TimeSpan remainTime = TimeSpan.FromSeconds(timeLimit);

        if(orderSuccess==true){ // 시간남았는데 이미 성공한 경우 조기종료
            timeLimit = -1;
            timerText.text = "00:00";
        }
        else if(timeLimit<6){ // 5초 이하일 때는 텍스트 색상 변경
            timerText.text = "<color=#DC143C>" + remainTime.ToString(@"mm\:ss") + "</color>";
        }
        else{
            timerText.text = remainTime.ToString(@"mm\:ss");
        }

        return timerText;
    }

    public void SetOrderText(){
        gameDataUnit = jsonManager.LoadJson<GameDataUnit>("Recipe");
        int randNum = UnityEngine.Random.Range(0, 21);
        recipeData = gameDataUnit.recipeArray[randNum];
        orderText.text = recipeData.nameKor.ToString() + "\n1잔 주세요!";
    }
//-----------------------필드, 선반 관련 함수--------------------------

    public void TouchShelfBtn(int floor)
    {
        StringBuilder path = new StringBuilder();
        path.Append("Images/TempImage/");

        switch (floor)
        {
            case 1:
                path.Append("1stShelf");
                TouchShelfFloor(path.ToString());
                break;
            case 2:
                path.Append("2ndShelf");
                TouchShelfFloor(path.ToString());
                break;
            case 3:
                path.Append("3rdShelf");
                TouchShelfFloor(path.ToString());
                break;
        }
    }

    void TouchShelfFloor(string getPath)
    {
        Sprite[] imageArray = Resources.LoadAll<Sprite>(getPath);
        
        SetShelfPopup(true);

        for(int i =0; i < itemBtnArray.Length; i++)
        {
            if(i < imageArray.Length)
                itemBtnArray[i].image.sprite = imageArray[i];
            else
            {
                itemBtnArray[i].gameObject.SetActive(false);
            }
        }
    }

    void SetShelfPopup(bool active)
    {
        popupParent.SetActive(active);
        popupBackGround.SetActive(active);
        popupBackGround.transform.SetAsFirstSibling();
        shelfPopupBoard.SetActive(active);
        ingredientPopupBoard.SetActive(false);
    }

    public void ExitPopup()
    {
        SetShelfPopup(false);
    }

    //선반 팝업창에서 재료를 선택했을 때 재료를 담을 것인지를 선택하는 팝업창 키기.
    public void TouchIngredientBtn()
    {
        SetIngredientPopup(true);
        nowIngredient = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
        ingredientImage.sprite = nowIngredient;
    }

    //active가 참이면 재료 팝업창을 키고 아니면 끄고
    void SetIngredientPopup(bool active)
    {
        ingredientPopupBoard.SetActive(active);
        if(active)
            popupBackGround.transform.SetSiblingIndex(1);
        else
            popupBackGround.transform.SetSiblingIndex(0);
    }

    public void TouchPickUpBtn()
    {
        SetIngredientPopup(false);
        SetFieldsArray();

    }

    public void TouchPutDownBtn()
    {
        SetIngredientPopup(false);
    }

    void SetFieldsArray()
    {
        //1. 필드에 아무것도 없으면 1번
        //2. 필드에 1개 있으면 2번
        //3. 필드에 2개 있으면 3번
        //4. 필드에 다 차있으면 1번을버리고 3번
        for(int i =0; i<3; i++)
        {
            if (!fieldsArray[i].isSpriteExist)
            {
                fieldsArray[i].fieldImage.sprite = nowIngredient;
                fieldsArray[i].isSpriteExist = true;
                break;
            }
            else if (fieldsArray[2].isSpriteExist)
            {
                fieldsArray[0].fieldImage.sprite = fieldsArray[1].fieldImage.sprite;
                fieldsArray[1].fieldImage.sprite = fieldsArray[2].fieldImage.sprite;
                fieldsArray[2].fieldImage.sprite = nowIngredient;
                break;
            }
        }
    }
// ---------------------------------------------------------
    /* #305 컵-드리기 버튼 눌렀을 때 성공여부확인
    public bool CheckOrderSuccess(){
        

        return orderSuccess
    }
    */

    /* 주문성공/실패에 따라 명성/재화 지급
    추후에 엑셀파일과 함께 작업
    public void SetGoldandReput(bool orderSuccess){
        int aftervalue=0;

        if(orderSuccess){ // 주문 성공시
            if(reputation>=0){ // 명성 >= 0
                gold += (정가 + 정가*명성*0.01);
            }
            else{ // 명성 < 0 : 추가금 X
                gold += 정가;
            }

            aftervalue=reputation+명성;
            if(aftervalue>100){
                reputation = 100;
            }
            else{
                reputation += 명성;
            }
            goldText.text = "G: " + gold.ToString();
            reputationText.text = "명성: " + reputation.ToString();
        }
        else{ // 주문 실패시
            gold += 정가; // cost

            aftervalue=reputation-명성; // 명성 하락 후의 값
            if(aftervalue<-100){
                reputation = -100;
            }
            else if(reputation<10 && reputation>-100){

            }
            else{
                reputation = -100;
            }
        }

    }*/

    public void ExitBtn(){    // MainScene-PauseBtn-게임종료
        jsonManager.SaveData<UserDataClass>(GameManager.instance.userData);
        Debug.Log("Data Save Complete");
        Application.Quit();
    }
}
