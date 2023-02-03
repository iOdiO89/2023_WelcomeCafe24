using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrologSceneManager : MonoBehaviour
{
    public void SkipBtn(){  // PrologScene-건너뛰기
        SceneManager.LoadScene("MainScene");
    }
}