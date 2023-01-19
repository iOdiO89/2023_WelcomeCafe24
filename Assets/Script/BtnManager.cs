using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnManager : MonoBehaviour
{

    public void StartNewBtn(){    // StartScene-새로하기
        SceneManager.LoadScene("PrologScene");
    }

    public void ExitBtn(){    // StartScene-종료하기 & MainScene-PauseBtn-게임종료
        Application.Quit();
    }

    public void SkipBtn(){  // PrologScene-건너뛰기
        SceneManager.LoadScene("MainScene");
    }


}