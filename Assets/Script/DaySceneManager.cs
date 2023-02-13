using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using UnityEngine.EventSystems;

public class DaySceneManager : MonoBehaviour
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
    [SerializeField]
    GameObject cupPopupParnet;
    [SerializeField]
    Image[] cupIngredientImageArr;
    [SerializeField]
    Image[] cupCapacityImageArr;
    [SerializeField]
    Button[] cupPlusBtnArr;
    [SerializeField]
    Button[] cupMinusBtnArr;

    private string[] cupIngredientNameArr;
    public int[] cupCapacityCountArr;          // (재료1의 개수, 재료2의 개수, 재료3의 개수)를 설정하는 배열.
    [HideInInspector]
    Sprite nowIngredient;

    public GameObject pausePopUp;
    public Text goldText;
    public Text reputationText;
    
    // 주문 타이머
    private float timeLimit;
    public Text timerText;

    // 주문
    
    private int orderCount; // 현재까지 처리한 주문 수
    private int orderSum; // 하루에 받아야하는 주문 수
    public Text orderText;
    private int orderIndex;
    private bool orderSuccess;
    [HideInInspector] public int successCount=0;
    [HideInInspector] public int failCount=0;
    [HideInInspector] public int todayCoin=0;
    [HideInInspector] public int todayReputation=0;

    //데이터 관련
    public JsonManager jsonManager;
    private GameDataUnit gameDataUnit;
    private Recipe recipeData;
    private Ingredient IngredientData;

    void Start(){
        cupCapacityCountArr = new int[3];
        for(int i =0; i<3; i++)
        {
            cupCapacityCountArr[i] = 0;
        }
        SetNewOrder();
    }

    void Update()
    {   
        SetValues();

        if(timeLimit>=0 && pausePopUp.activeSelf==false && orderSuccess==false){
            CountDownTimer();
        }
        if(orderSuccess){
            timerText.text = "00:00";
            timeLimit = 10;
        }
   }

    public void TempBtn(){
        GameManager.instance.userData.gold += 10;
        GameManager.instance.userData.reputation += 10;
        jsonManager.SaveData<UserDataClass>(GameManager.instance.userData);
    }

    //명성, 재화 화면에 출력
    public void SetValues(){
        goldText.text = "G: " + GameManager.instance.userData.gold.ToString();
        reputationText.text = "명성: " + GameManager.instance.userData.reputation.ToString();
    }


    //90초 주문 타이머
    public void CountDownTimer(){
        timeLimit -= Time.deltaTime; // 1초씩 제거
        TimeSpan remainTime = TimeSpan.FromSeconds(timeLimit);

        if(timeLimit<6){ // 5초 이하일 때는 텍스트 색상 변경
            timerText.text = "<color=#DC143C>" + remainTime.ToString(@"mm\:ss") + "</color>";
        }
        else{
            timerText.text = remainTime.ToString(@"mm\:ss");
        }
    }

    //하루에 처리해야하는 주문 수 확인
    private void CheckOrderCount(){
            if(GameManager.instance.userData.reputation<=30){ // 0 <= 명성 <= 30
                orderSum = 5;
            }
            else if(GameManager.instance.userData.reputation<=60){ // 30 < 명성 <= 60
                orderSum = 6;
            } 
            else if(GameManager.instance.userData.reputation<=90){ // 60 < 명성 <= 90
                orderSum = 7;
            }
            else{ // 90 < 명성 <= 100
                orderSum = 8;
            }
    }

    public void SetNewOrder(){
        timeLimit = 90;
        PrintOrderText();
        orderCount++;

        for(int i=0; i<10; i++){ // 컵 비우기
            TouchCupMinusBtn(0);
            TouchCupMinusBtn(1);
            TouchCupMinusBtn(2);
        }
    }

    public void PrintOrderText(){
        orderText.text = MakeOrderText() + "\n1잔 주세요!";
    }

    private string MakeOrderText(){
        gameDataUnit = jsonManager.LoadJson<GameDataUnit>("Recipe");
        while(true){
            orderIndex = UnityEngine.Random.Range(0, 21);
            if(GameManager.instance.userData.recipeUnlock[orderIndex])
                break;
        }
        recipeData = gameDataUnit.recipeArray[orderIndex];
        return recipeData.nameKor;
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

//-----------------------컵 함수----------------------------
    public void TouchCupBtn()
    {   
        cupPopupParnet.SetActive(true);
        for(int i =0; i<3; i++)
        {
            if (fieldsArray[i].isSpriteExist)
            {
                cupPlusBtnArr[i].enabled = true;
                cupMinusBtnArr[i].enabled = true;
                cupIngredientImageArr[i].color = new Color(1, 1, 1);
                cupIngredientImageArr[i].sprite = fieldsArray[i].fieldImage.sprite;
            }
            else
            {
                cupPlusBtnArr[i].enabled= false;
                cupMinusBtnArr[i].enabled= false;
            }
        }
    }

    public void TouchCupPlusBtn(int btnIdx)
    {
        int totalCapacity = 0;
        for(int i=0; i<3; i++)
        {
            totalCapacity += cupCapacityCountArr[i];
        }

        //재료가 10개 차있으면 리턴
        if (totalCapacity >= 10) return;
        else
        {
            switch (btnIdx)
            {
                case 1:
                    cupCapacityCountArr[0]++;
                    SetCupCapacity();
                    break;
                case 2:
                    cupCapacityCountArr[1]++;
                    SetCupCapacity();
                    break;
                case 3:
                    cupCapacityCountArr[2]++;
                    SetCupCapacity();
                    break;
            }
        }
    }

    public void TouchCupMinusBtn(int btnIdx)
    {
        int totalCapacity = 0;
        for (int i = 0; i < 3; i++)
        {
            totalCapacity += cupCapacityCountArr[i];
        }

        //아무것도 안들어 있으면 리턴
        if (totalCapacity <= 0) return;
        else
        {
            switch (btnIdx)
            {
                case 1:
                    if (cupCapacityCountArr[0] == 0)
                        return;
                    cupCapacityCountArr[0]--;
                    SetCupCapacity();
                    break;
                case 2:
                    if (cupCapacityCountArr[1] == 0)
                        return;
                    cupCapacityCountArr[1]--;
                    SetCupCapacity();
                    break;
                case 3:
                    if (cupCapacityCountArr[2] == 0)
                        return;
                    cupCapacityCountArr[2]--;
                    SetCupCapacity();
                    break;
            }
        }
    }

    void SetCupCapacity()
    {
        int capacityIdx = 0;
        int targetIdx = cupCapacityCountArr[0];
        for (int i =0; i< cupCapacityImageArr.Length; i++)
        {
            cupCapacityImageArr[i].color = new Color(1, 1, 1);
        }

        //재료 1이 얼마나 있는지 확인 -> 해당 개수만큼 색칠 -> 재료 3까지 반복
        //i는 재료 인덱스, capacityIdx는 현재 색칠해야할 이미지 인덱스
        //아직 재료 색깔은 안받아서 그냥 rgb로 설정
        for (int i=0; i<cupCapacityCountArr.Length; i++)
        {
            for (; capacityIdx < targetIdx; capacityIdx++)
            {
                switch(i)
                {
                    case 0:
                        //Debug.Log("1: " + capacityIdx);
                        cupCapacityImageArr[capacityIdx].color = new Color(1, 0, 0);
                        break;
                    case 1:
                        //Debug.Log("2: " + capacityIdx);
                        cupCapacityImageArr[capacityIdx].color = new Color(0, 1, 0);
                        break;
                    case 2:
                        //Debug.Log("3: " + capacityIdx);
                        cupCapacityImageArr[capacityIdx].color = new Color(0, 0, 1);
                        break;
                }
            }
            if(i<2)
                targetIdx += cupCapacityCountArr[i + 1];
        }
    }

    public void CupGiveBtn(){ // #305 컵-드리기 버튼
        bool check = CheckOrderSuccess();
        if(check){
            Debug.Log("레시피 제작 성공!");
        }
        else{
            Debug.Log("레시피 제작 실패");
        }
        SetGoldandReput(check);

        GameObject cupPopUp = GameObject.Find("CupPopUp");
        cupPopUp.SetActive(false);

        if(orderCount != orderSum){
            Invoke("SetNewOrder", 1.5f); // 1.5초 후 새로운 주문 시작
            //SetNewOrder();
        }
        else{ // 낮에 처리해야할 주문이 모두 끝난 경우

        }
    }

    // 주문이 맞는지 확인
    private bool CheckOrderSuccess(){ 
        List<Tuple<int, int>> ingAnswerList = new List<Tuple<int, int>>(); // 정답
        ingAnswerList.Add(new Tuple<int, int>(recipeData.ing1Index, recipeData.ing1Ratio));
        ingAnswerList.Add(new Tuple<int, int>(recipeData.ing2Index, recipeData.ing2Ratio));
        ingAnswerList.Add(new Tuple<int, int>(recipeData.ing3Index, recipeData.ing3Ratio));

        List<Tuple<int, int>> ingList = new List<Tuple<int, int>>(); // 사용자가 입력한 답
        int temp;
        for(int i=0; i<3; i++){
            // 재료의 index, 비율을 tuple로 저장
            if(fieldsArray[i].fieldImage.sprite == null){ // null 인 경우
                //Debug.Log("it's Null!!!!");
                ingList.Add(new Tuple<int, int>(-1, 0)); 
            }
            else{
                temp = System.Convert.ToInt32(fieldsArray[i].fieldImage.sprite.name);
                ingList.Add(new Tuple<int, int>(temp, cupCapacityCountArr[i])); 
            }
        }

        // Item1기준으로 오름차순 정렬
        ingAnswerList.Sort((a, b) => a.Item1.CompareTo(b.Item1));
        ingList.Sort((a, b) => a.Item1.CompareTo(b.Item1));

        for(int i=0; i<3; i++){
            Debug.Log($"answer : {ingAnswerList[i].Item1} : {ingAnswerList[i].Item2} / user : {ingList[i].Item1} : {ingList[i].Item2}");
        }

        for(int i=0; i<3; i++){
            if(!ingAnswerList[i].Equals(ingList[i])){
                return false;
            }
        }
        return true;
    }

    // 주문성공/실패에 따라 명성/재화 지급
    private void SetGoldandReput(bool orderSuccess){ 
        if(orderSuccess){ // 주문 성공시
            successCount++;

            // 돈
            double tempPrice;
            if(GameManager.instance.userData.reputation<0){
                Debug.Log("가격 *1");
                tempPrice = 1.0;   
            }
            else if(GameManager.instance.userData.reputation<=30){ // 0 <= 명성 <= 30
                Debug.Log("가격 *1.3");
                tempPrice = (double)recipeData.price * 1.3;
            }
            else if(GameManager.instance.userData.reputation<=60){ // 30 < 명성 <= 60
                Debug.Log("가격 *1.6");
                tempPrice = (double)recipeData.price * 1.6;
            } 
            else if(GameManager.instance.userData.reputation<=90){ // 60 < 명성 <= 90
                Debug.Log("가격 *1.9");
                tempPrice = (double)recipeData.price * 1.9;
            }
            else{ // 90 < 명성 <= 100
                Debug.Log("가격 *2.5");
                tempPrice = (double)recipeData.price * 2.5;
            }
            todayCoin += (int)tempPrice;
            GameManager.instance.userData.gold += (int)tempPrice;

            //명성
            int tempReputation = UnityEngine.Random.Range(0, 2);
            todayReputation += tempReputation;
            GameManager.instance.userData.reputation += tempReputation;
        }
        else{ // 주문 실패시
            failCount++;

            //명성
            GameManager.instance.userData.reputation -= UnityEngine.Random.Range(0, 2);
        }
    }

// ---------------------------------------------------------
    /*
    public void ExitBtn(){    // MainScene-PauseBtn-게임종료
        jsonManager.SaveData<UserDataClass>(GameManager.instance.userData);
        Debug.Log("Data Save Complete");
        Application.Quit();
    }*/
}
