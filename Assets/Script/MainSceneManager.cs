using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MainSceneManager : MonoBehaviour
{      
    // 재화, 명성
    int gold=0;
    int reputation=0;
    public Text goldText;
    public Text reputationText;

    // 주문 타이머
    public float timeLimit;
    public Text timerText;

    // 주문
    public int orderCount;
    public Text orderText;
    bool orderSuccess;
    
    void Start()
    {
        goldText.text = "G: " + gold.ToString();
        reputationText.text = "명성: " + reputation.ToString();
    }

    void Update()
    { 
        if(timeLimit>=0){
            CountDownTimer();
        }
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
}
