using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DaySceneManager : MonoBehaviour
{
    [SerializeField] GameObject popupParent;
    [SerializeField] GameObject popupBackGround;
    [SerializeField] GameObject shelfPopupBoard;
    [SerializeField] Button[] itemBtnArray;
    [SerializeField] GameObject ingredientPopupBoard;
    [SerializeField] Image ingredientImage;
    [SerializeField] Text ingredientDetailText;
    [SerializeField] Text ingredientNameText;
    [SerializeField] Field[] fieldsArray;
    [SerializeField] GameObject cupPopupParnet;
    [SerializeField] Image[] cupIngredientImageArr;
    [SerializeField] Image[] cupCapacityImageArr;
    [SerializeField] Button[] cupPlusBtnArr;
    [SerializeField] Button[] cupMinusBtnArr;
    [SerializeField] Button cup;

    //private string[] cupIngredientNameArr;
    // (재료1의 개수, 재료2의 개수, 재료3의 개수)를 설정하는 배열.
    private int totalCapacity;
    public int[] cupCapacityCountArr;
    [HideInInspector]
    Sprite nowIngredient;

    public GameObject pausePopUp;
    public Text goldText;
    public Text reputationText;
    
    // 주문 타이머
    private float timeLimit;
    public Text timerText;

    // 주문
    public GameObject order;
    private int orderCount; // 현재까지 처리한 주문 수
    private int orderSum; // 하루에 받아야하는 주문 수
    public Text orderText;
    private int orderIndex;
    private bool orderSuccess;
    [HideInInspector] public static int successCount=0;
    [HideInInspector] public static int failCount=0;
    [HideInInspector] public static int todayCoin=0;
    [HideInInspector] public static int todayReputation=0;

    public Text dayText;
    public Text dayFadeText;

    public List<GameObject> menuList = new List<GameObject>();
    [SerializeField] private GameObject recipeObject;
    [SerializeField] private Text recipeTitleText;
    [SerializeField] private Text recipeDetailText;
    public List<Text> menuTextList = new List<Text>();
    private Color transparentColor;
    [SerializeField] private GameObject[] fieldOutline;

    //데이터 관련
    public JsonManager jsonManager;
    private Recipe recipeData;
    private Ingredient ingredientData;

    [SerializeField] private Slider totalVolumeSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider effectSlider;

    private NoticeUI notice;
    private FadeUI fade;

    void Awake(){
        GameManager.instance.daySceneActive = GameManager.instance.daySceneActive? false : true;
    }

    void Start(){
        if(GameManager.instance.daySceneActive){
            if(!GameManager.instance.continueBool) GameManager.instance.userData.day++;
            Debug.Log($"Day {GameManager.instance.userData.day} - 낮");
            order.SetActive(true);
            CheckOrderCount();
            timeLimit = 90;
            PrintOrderText(); 
        }
        else{
            Debug.Log($"Day {GameManager.instance.userData.day} - 밤");
            orderSum = 5;
            order.SetActive(false);
        }

        SetRecipeColor();
        PrintDayText();
        notice = FindObjectOfType<NoticeUI>();
        fade = FindObjectOfType<FadeUI>();
        fade.FadeIn();
        orderCount++;
        
        ColorBlock color = cup.colors;
        color.normalColor = SetColor();
        cup.colors = color;
        cupCapacityCountArr = new int[3];
        for(int i =0; i<3; i++)
        {
            cupCapacityCountArr[i] = 0;
        }

        todayCoin = 0;
        todayReputation = 0;
        successCount = 0;
        failCount = 0;
    }

    void Update()
    {   
        SetValues();
        if(GameManager.instance.daySceneActive){
            if(timeLimit>=0 && pausePopUp.activeSelf==false && orderSuccess==false){
                CountDownTimer();
            }
        }
   }

    //명성, 재화 화면에 출력
    private void SetValues(){
        goldText.text = "G: " + GameManager.instance.userData.gold.ToString();
        reputationText.text = "명성: " + GameManager.instance.userData.reputation.ToString();
    }

    //90초 주문 타이머
    public void CountDownTimer(){
        timeLimit -= Time.deltaTime;
        TimeSpan remainTime = TimeSpan.FromSeconds(timeLimit);

        if(timeLimit < 1){
            CupGiveBtn();
            timerText.text = "00:00";
        }
        else if(timeLimit<6){ // 5초 이하일 때는 텍스트 색상 변경
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
                Debug.Log("오늘 주문 수 : 5");
            }
            else if(GameManager.instance.userData.reputation<=60){ // 30 < 명성 <= 60
                orderSum = 6;
                Debug.Log("오늘 주문 수 : 6");
            } 
            else if(GameManager.instance.userData.reputation<=90){ // 60 < 명성 <= 90
                orderSum = 7;
                Debug.Log("오늘 주문 수 : 7");
            }
            else{ // 90 < 명성 <= 100
                orderSum = 8;
                Debug.Log("오늘 주문 수 : 8");
            }
    }

    // 새 주문 설정
    public void SetNewOrder(){
        if(GameManager.instance.daySceneActive){
            timeLimit = 90;
            PrintOrderText();
        }
        //Debug.Log($"완료한 주문 수 : {orderCount}");
        orderCount++;
        ClearIngredientImages();
        ClearFields();
        ClearCupCapacityImages();
    }

    public void PrintOrderText(){
        orderText.text = MakeOrderText() + "\n1잔 주세요!";
    }

    private string MakeOrderText(){
        while(true){
            orderIndex = UnityEngine.Random.Range(0, 21);
            if(GameManager.instance.userData.recipeUnlock[orderIndex]>0)
                break;
        }
        recipeData = GameManager.instance.gameDataUnit.recipeArray[orderIndex];
        return recipeData.nameKor;
    }


//-----------------------필드, 선반 관련 함수--------------------------

    public void TouchShelfBtn(int floor)
    {
        SoundManager.instance.PlayEffect("shelf");
        StringBuilder path = new StringBuilder();
        path.Append("Images/Both/Ingredient/");

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
        
        int ingredientIndex;
        SetShelfPopup(true);
        for(int i =0; i < itemBtnArray.Length; i++)
        {
            if (i < imageArray.Length)
            {
                ingredientIndex = int.Parse(imageArray[i].name);
                itemBtnArray[i].gameObject.SetActive(true);
                itemBtnArray[i].image.sprite = imageArray[i];

                if (!GameManager.instance.userData.ingredientUnlock[ingredientIndex])
                {
                    itemBtnArray[i].enabled = false;
                    // itemBtnArray[i].image.color = new Color(0.5f, 0.5f, 0.5f);
                }
                else
                {
                    itemBtnArray[i].enabled = true;
                    itemBtnArray[i].image.color = SetColor();   
                }
            }
            else
            {
                itemBtnArray[i].gameObject.SetActive(false);
            }
        }
    }
    
    private Color SetColor(){
        if(GameManager.instance.daySceneActive)
            return new Color(235/255f, 226/255f, 116/255f);
        else
            return new Color(70/255f, 130/255f, 180/255f);
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
        SoundManager.instance.PlayEffect("popup");
        SetShelfPopup(false);
    }

    //선반 팝업창에서 재료를 선택했을 때 재료를 담을 것인지를 선택하는 팝업창 키기.
    public void TouchIngredientBtn()
    {
        SoundManager.instance.PlayEffect("click");
        SetIngredientPopup(true);
        nowIngredient = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
        int ingredientIndex = System.Convert.ToInt32(nowIngredient.name);
        ingredientImage.sprite = nowIngredient; 
        ingredientImage.color = SetColor();       
        SetIngredientText();
    }

    void SetIngredientText()
    {
        ingredientDetailText.text = "";
        ingredientNameText.text = "";
        int ingredientIndex = int.Parse(ingredientImage.sprite.name);
        
        ingredientNameText.text = GameManager.instance.gameDataUnit.ingredientArray[ingredientIndex].nameKor;
        ingredientDetailText.text = GameManager.instance.gameDataUnit.ingredientArray[ingredientIndex].detail;
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
        SoundManager.instance.PlayEffect("click");
        SetIngredientPopup(false);
        SetFieldsArray();
    }

    public void TouchPutDownBtn()
    {
        SoundManager.instance.PlayEffect("click");
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
                fieldsArray[i].fieldImage.color = SetColor();
                break;
            }
            else if (fieldsArray[2].isSpriteExist)
            {
                fieldsArray[0].fieldImage.sprite = fieldsArray[1].fieldImage.sprite;
                fieldsArray[1].fieldImage.sprite = fieldsArray[2].fieldImage.sprite;
                fieldsArray[2].fieldImage.sprite = nowIngredient;
                for(int j=0; j<3; j++){
                    fieldsArray[j].fieldImage.color = SetColor();
                }
                break;
            }
        }
    }

//-----------------------컵 함수----------------------------
    public void TouchCupBtn()
    {   
        SoundManager.instance.PlayEffect("button");
        cupPopupParnet.SetActive(true);
        for(int i =0; i<3; i++)
        {
            if (fieldsArray[i].isSpriteExist)
            {
                cupPlusBtnArr[i].enabled = true;
                cupMinusBtnArr[i].enabled = true;
                cupIngredientImageArr[i].color = SetColor();
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
        SoundManager.instance.PlayEffect("water");
        totalCapacity = 0;
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
        SoundManager.instance.PlayEffect("water");
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
                        cupCapacityImageArr[capacityIdx].color = new Color(255/255f, 217/255f, 61/255f);
                        break;
                    case 1:
                        //Debug.Log("2: " + capacityIdx);
                        cupCapacityImageArr[capacityIdx].color = new Color(79/255f, 32/255f, 13/255f);
                        break;
                    case 2:
                        //Debug.Log("3: " + capacityIdx);
                        cupCapacityImageArr[capacityIdx].color = new Color(255/255f, 132/255f, 0);
                        break;
                }
            }
            if(i<2)
                targetIdx += cupCapacityCountArr[i + 1];
        }
    }

    public void CupGiveBtn(){ // #305 컵-드리기 버튼
        SoundManager.instance.PlayEffect("order");
        if(GameManager.instance.daySceneActive){ // 낮일 때
            var returnValue = CheckOrderSuccess();
            bool check = returnValue.Item1;
            if(check){
                Debug.Log("레시피 제작 성공!");
            }
            else{
                Debug.Log("레시피 제작 실패");
            }
            notice.SUB(returnValue.Item2);
            SetGoldandReput(check);
            cupPopupParnet.SetActive(false);

            if(orderCount != orderSum){
                //Invoke("SetNewOrder", 1.5f); // 1.5초 후 새로운 주문 시작
                SetNewOrder();
            }
            else{ // 낮에 처리해야할 주문이 모두 끝난 경우
                jsonManager.SaveData(GameManager.instance.userData);
                Debug.Log("Data save Complete");
                SceneManager.LoadScene("EveningScene");
            }
        }
        else{ // 밤일 때
            int totalCapacity = 0;
            for (int i = 0; i < 3; i++){
                totalCapacity += cupCapacityCountArr[i];
            }
            if(totalCapacity!=10) {
                notice.SUB("비율의 총합은 10을 맞춰주세요!");
                return;
            }

            var returnValue = CheckOrderSuccessNight();
            if(returnValue.Item1){
                GameManager.instance.userData.recipeUnlock[returnValue.Item3] = 3;
                jsonManager.SaveData(GameManager.instance.userData);
                SetRecipeColor();
            }
            cupPopupParnet.SetActive(false);
            notice.SUB(returnValue.Item2);

            if(orderCount != orderSum){
                SetNewOrder();
            }
            else{ // 밤에 처리해야할 주문이 모두 끝난 경우
                Invoke("ChangeSceneNightToDay", 0.5f);
            }
        }
    }

    private void ChangeSceneNightToDay(){
        jsonManager.SaveData(GameManager.instance.userData);
        Debug.Log("Data save Complete");
        fade.FadeOut();
        SceneManager.LoadScene("DayScene");
    }

    // 낮 - 주문이 맞는지 확인
    private (bool, string) CheckOrderSuccess(){ 
        List<Tuple<int, int>> ingAnswerList = new List<Tuple<int, int>>(); // 정답
        ingAnswerList.Add(new Tuple<int, int>(recipeData.ing1Index, recipeData.ing1Ratio));
        ingAnswerList.Add(new Tuple<int, int>(recipeData.ing2Index, recipeData.ing2Ratio));
        ingAnswerList.Add(new Tuple<int, int>(recipeData.ing3Index, recipeData.ing3Ratio));
        ingAnswerList.Sort((a, b) => a.Item1.CompareTo(b.Item1));

        List<Tuple<int, int>> ingList = new List<Tuple<int, int>>(); // 사용자가 입력한 답
        int temp;
        for(int i=0; i<3; i++){
            // 재료의 index, 비율을 tuple로 저장
            if(fieldsArray[i].fieldImage.sprite.name == "default"){
                ingList.Add(new Tuple<int, int>(-1, 0)); 
            }
            else{
                temp = System.Convert.ToInt32(fieldsArray[i].fieldImage.sprite.name);
                ingList.Add(new Tuple<int, int>(temp, cupCapacityCountArr[i])); 
            }
        }
        ingList.Sort((a, b) => a.Item1.CompareTo(b.Item1));

        for(int i=0; i<3; i++){
            if(!ingAnswerList[i].Equals(ingList[i])){
                return (false, $"{recipeData.nameKor} 을(를) 제조하는데 실패했습니다!");
            }
        }
        return (true, $"{recipeData.nameKor} 을(를) 제조하는데 성공했습니다!");
    }

    // 밤 - 주문이 맞는지 확인
    private (bool, string, int) CheckOrderSuccessNight(){
        List<Tuple<int, int, int>> answerIngIndexList = new List<Tuple<int, int, int>>(); //레시피 재료 비율
        List<Tuple<int, int, int>> answerIngRatioList = new List<Tuple<int, int, int>>();
        for(int i=0; i<21; i++){
            recipeData = GameManager.instance.gameDataUnit.recipeArray[i];
            answerIngIndexList.Add(new Tuple<int, int, int>(recipeData.ing1Index, recipeData.ing2Index, recipeData.ing3Index));
            answerIngRatioList.Add(new Tuple<int, int, int>(recipeData.ing1Ratio, recipeData.ing2Ratio, recipeData.ing3Ratio));
        }
        
        List<Tuple<int, int>> ingList = new List<Tuple<int, int>>(); // 사용자가 입력한 답
        int temp;
        for(int i=0; i<3; i++){
            // 재료의 index, 비율을 tuple로 저장
            if(fieldsArray[i].fieldImage.sprite.name != "default"){
                temp = System.Convert.ToInt32(fieldsArray[i].fieldImage.sprite.name);
                ingList.Add(new Tuple<int, int>(temp, cupCapacityCountArr[i])); 
                //ingList.Add(new Tuple<int, int>(-1, 0)); 
            }
        }
        ingList.Sort((a, b) => a.Item1.CompareTo(b.Item1));
        for(int i=ingList.Count; i<3; i++){
            ingList.Add(new Tuple<int, int>(-1, 0)); 
        }

        Tuple<int, int, int> userIngIndex = new Tuple<int, int, int>(ingList[0].Item1, ingList[1].Item1, ingList[2].Item1);
        Tuple<int, int, int> userIngRatio = new Tuple<int, int, int>(ingList[0].Item2, ingList[1].Item2, ingList[2].Item2);
        
        for(int i=0; i<21; i++){
            if(answerIngIndexList[i].Equals(userIngIndex)){
                recipeData = GameManager.instance.gameDataUnit.recipeArray[i];
                if(answerIngRatioList[i].Equals(userIngRatio)){ // 모두 일치
                    SoundManager.instance.PlayEffect("success");
                    return (true, $"{recipeData.nameKor} 을(를) 제조하는데 성공했습니다!", i);
                }

                Debug.Log($"answerIngIndexList = {answerIngIndexList[i].Item1}, {answerIngIndexList[i].Item2}, {answerIngIndexList[i].Item3}");
                Debug.Log($"userIngIndex = {userIngIndex.Item1}, {userIngIndex.Item2}, {userIngIndex.Item3}");
                Debug.Log($"answerIngRatioList = {answerIngRatioList[i].Item1}, {answerIngRatioList[i].Item2}, {answerIngRatioList[i].Item3}");
                Debug.Log($"userIngRatio = {userIngRatio.Item1}, {userIngRatio.Item2}, {userIngRatio.Item3}");

                string temp1 = "", temp2 = "";
                ingredientData = GameManager.instance.gameDataUnit.ingredientArray[userIngIndex.Item1];
                Ingredient ingredientData2 = GameManager.instance.gameDataUnit.ingredientArray[userIngIndex.Item2];
                if(userIngRatio.Item2 != answerIngRatioList[i].Item2){
                    if(userIngRatio.Item2 > answerIngRatioList[i].Item2) temp2 = " " + ingredientData2.high1;
                    else temp2 = " " + ingredientData2.low1;

                    if(userIngRatio.Item1 == answerIngRatioList[i].Item1) temp1 = "";
                    else if(userIngRatio.Item1 > answerIngRatioList[i].Item1) temp1 = ingredientData.high1;
                    else temp1 = ingredientData.low1;
                }
                else{
                    if(userIngRatio.Item1 > answerIngRatioList[i].Item1) temp1 = ingredientData.high2;
                    else temp1 = ingredientData.low2;
                }
                SoundManager.instance.PlayEffect("fail");
                return (false, $"{temp1}{temp2} {recipeData.nameKor}가 제조되었습니다.(제조실패)", -1);
            }
        }
        SoundManager.instance.PlayEffect("fail");
        return (false, "이도저도 아닌 무언가를 만들었습니다(제조실패)", -1);
    }

    // 주문성공/실패에 따라 명성/재화 지급
    private void SetGoldandReput(bool orderSuccess){ 
        if(orderSuccess){ // 주문 성공시
            successCount++;

            // 돈
            double tempPrice;
            if(GameManager.instance.userData.reputation<0){
                tempPrice = 1.0;   
            }
            else if(GameManager.instance.userData.reputation<=30){ // 0 <= 명성 <= 30
                tempPrice = (double)recipeData.price * 1.3;
            }
            else if(GameManager.instance.userData.reputation<=60){ // 30 < 명성 <= 60
                tempPrice = (double)recipeData.price * 1.6;
            } 
            else if(GameManager.instance.userData.reputation<=90){ // 60 < 명성 <= 909
                tempPrice = (double)recipeData.price * 1.9;
            }
            else{ // 90 < 명성 <= 100
                tempPrice = (double)recipeData.price * 2.5;
            }
            todayCoin += (int)tempPrice;
            GameManager.instance.userData.gold += (int)tempPrice;

            //명성
            int tempReputation = UnityEngine.Random.Range(1, 3);
            todayReputation += tempReputation;
            GameManager.instance.userData.reputation += tempReputation;
        }
        else{ // 주문 실패시
            failCount++;

            //명성
            int tempReputation = UnityEngine.Random.Range(1, 3);
            todayReputation -= tempReputation;
            GameManager.instance.userData.reputation -= tempReputation;
        }
    }

    void ClearIngredientImages()
    {
        for(int i =0; i<cupIngredientImageArr.Length; i++)
        {
            cupIngredientImageArr[i].sprite = Resources.Load<Sprite>("Images/Both/default");
            cupIngredientImageArr[i].color = new Color(0, 0, 0);
        }
        
        for(int i=0; i<cupCapacityCountArr.Length;i++)
        {
            cupCapacityCountArr[i] = 0;
        }
    }

    void ClearFields()
    {
        for(int i =0; i<fieldsArray.Length; i++)
        {
            fieldsArray[i].isSpriteExist = false;
            fieldsArray[i].fieldImage.sprite = Resources.Load<Sprite>("Images/Both/default");
            Color color = new Color(1f, 1f, 1f);
            fieldsArray[i].fieldImage.color = color;
        }
    }

    void ClearCupCapacityImages()
    {
        for(int i=0; i< cupCapacityImageArr.Length; i++)
        {
            cupCapacityImageArr[i].color = new Color(1, 1, 1);
        }
    }

//-------------------------- 레시피 ----------------------------------------
    public void SetRecipeColor(){
        transparentColor = menuTextList[0].color;
        transparentColor.a = 0.3f;
        for(int i=0; i<21; i++){
            if(GameManager.instance.userData.recipeUnlock[i]==0){
                menuTextList[i].color = transparentColor;
            }
            else{
                menuTextList[i].color = menuTextList[0].color;
            }
        }
    }

    public void ShowRecipe(int index){
        recipeObject.SetActive(true);
        int recipeSize = 21;
        for(int i=0; i<recipeSize; i++){
            if(i==index){
                recipeData = GameManager.instance.gameDataUnit.recipeArray[i];

                if(GameManager.instance.userData.recipeUnlock[i]==0){
                    recipeTitleText.text = recipeData.nameKor;
                    recipeDetailText.text = "아직 모르는\n레시피입니다.";
                }
                else{
                    string tempText;
                    if(GameManager.instance.userData.recipeUnlock[i]==1){
                        tempText = recipeData.level1Detail;                    
                    }
                    else if(GameManager.instance.userData.recipeUnlock[i]==2){
                        tempText = recipeData.level2Detail;   
                    }
                    else{
                        tempText = recipeData.level3Detail;    
                    }

                    recipeTitleText.text = recipeData.nameKor;

                    string recipeDetail = "";
                    string[] words = tempText.Split(',');
                    for(int j=0; j<words.Length; j++){
                        recipeDetail += words[j];
                        if(j!=words.Length-1) recipeDetail+="\n";
                    }
                    recipeDetailText.text = recipeDetail;
                } 
            }
        }
    }

    public void MenuBtn(){
        SoundManager.instance.PlayEffect("button");
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        string name = clickObject.name;
        string stringRes = name.Substring(name.Length-2);
        int intRes=-1;
        if(!int.TryParse(stringRes, out intRes)){ // 두자리수일때
            stringRes = name.Substring(name.Length-1);
        }
        //print(System.Convert.ToInt32(stringRes) + "번 메뉴");
        ShowRecipe(System.Convert.ToInt32(stringRes));
    }

// -------------- 일시정지 팝업창 -------------------------------------------
    
    public void PrintDayText(){
        dayText.text = "DAY " + GameManager.instance.userData.day.ToString();
        if(GameManager.instance.daySceneActive){
            dayFadeText.text = "DAY " + GameManager.instance.userData.day.ToString() + " - 낮";
        }
        else{
            dayFadeText.text = "DAY " + GameManager.instance.userData.day.ToString() + " - 밤";
        }
    }
    
    public void ExitBtn(){    // dayScene-PauseBtn-게임종료
        SoundManager.instance.PlayEffect("button");
        jsonManager.SaveData(GameManager.instance.userData);
        Debug.Log("Data save Complete");
        Application.Quit();
    }

    public void BtnSound(){
        SoundManager.instance.PlayEffect("button");
    }

    public void ChangeTotalVolume(){
        float temp = totalVolumeSlider.value*0.6f;
        GameManager.instance.userData.bgmVolume = temp;
        bgmSlider.value = temp;
        SoundManager.instance.ChangeBGMVolume(temp);

        GameManager.instance.userData.effectVolume = totalVolumeSlider.value;
        effectSlider.value = totalVolumeSlider.value;
    }

    public void ChangeBGMVolume(){
        GameManager.instance.userData.bgmVolume = bgmSlider.value;
        SoundManager.instance.ChangeBGMVolume(bgmSlider.value);
    }

    public void ChangeEffectVolume(){
        GameManager.instance.userData.effectVolume = effectSlider.value;
    }

    public void FinishBtn(){ // tempBtn 나중에 지울 예정
        jsonManager.SaveData(GameManager.instance.userData);
        SceneManager.LoadScene("EveningScene");
    }

    public void Finish2Btn(){ // tempBtn 나중에 지울 예정
        jsonManager.SaveData(GameManager.instance.userData);
        Debug.Log("Data save Complete");
        fade.FadeOut();
        SceneManager.LoadScene("DayScene");
    }
}
