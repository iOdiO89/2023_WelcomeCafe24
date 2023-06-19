using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    JsonManager jsonManager;

    [SerializeField]
    TutorialScriptWrapper tutorialScriptWrapper;
    [SerializeField]
    string tutorialScriptName;

    [SerializeField]
    Transform canvasTransform;
    [SerializeField]
    Transform fadeImageTransform;
    [SerializeField]
    GameObject tutorialTextParent;
    [SerializeField]
    Text tutorialText;
    [SerializeField]
    FadeUI fade;
    [SerializeField]
    Transform brightObjectsTransform;

    //����� �κ�
    [SerializeField]
    GameObject[] brightObjectMaskArray;
    [SerializeField]
    GameObject pauseBtn;

    [SerializeField]
    GameObject dayBg;
    [SerializeField]
    GameObject nightBg;

    [HideInInspector]
    bool isScriptEnd = true;
    bool isScriptPrinting = false;
    int nowScriptIndex = 0;
    Coroutine nowCoroutine;

    IEnumerator Start()
    {
        jsonManager = new JsonManager();

        tutorialScriptWrapper = jsonManager.LoadJson<TutorialScriptWrapper>(tutorialScriptName);
        tutorialScriptWrapper.Parse();

        fade.TutorialFadeIn(0.4f);
        yield return new WaitForSeconds(1.5f);
        SetBrightObjectSlbling(tutorialTextParent);
        SetBrightObjectSlbling(pauseBtn);
        ScreenTouchEvent();
    }


    public void ScreenTouchEvent()
    {
        if (!isScriptEnd)
            PrintDialogue();

        else if (isScriptEnd)
        {
            if (tutorialScriptWrapper == null)
                return;

            isScriptEnd = false;
            PrintDialogue();
        }
    }

    void PrintDialogue()
    {
        if (!isScriptPrinting)
        {
            if (nowScriptIndex == tutorialScriptWrapper.tutorialScriptArray.Length)
            {
                Debug.Log("DialogueEnd");
                isScriptEnd = true;
                return;
            }

            if (nowScriptIndex < tutorialScriptWrapper.tutorialScriptArray.Length)
            {
                TutorialScript nowScript = tutorialScriptWrapper.tutorialScriptArray[nowScriptIndex];
                // Debug.Log("PrintDialog: " + nowScriptIndex + "/" + tutorialScriptWrapper.tutorialScriptArray.Length);

                //Event�� �����ϸ� ����
                if (nowScript.tutorialEvent != TutorialEvent.None)
                {
                    SetAction(nowScript.tutorialEvent);
                }

                if (!isScriptEnd && !isScriptPrinting)
                {
                    tutorialText.text = "";
                    nowCoroutine = StartCoroutine(PrintDialogueText());
                }

                nowScriptIndex++;
            }
        }
        else if (isScriptPrinting)
        {
            SpreadDialogue(tutorialScriptWrapper.tutorialScriptArray[nowScriptIndex - 1]);
        }
    }

    IEnumerator PrintDialogueText()
    {
        isScriptPrinting = true;
        TutorialScript nowScript = tutorialScriptWrapper.tutorialScriptArray[nowScriptIndex];

        for (int i = 0; i < nowScript.tutorialScript.Length; i++)
        {
            tutorialText.text += nowScript.tutorialScript[i];
            yield return new WaitForSeconds(0.07f);
        }

        isScriptPrinting = false;
    }

    void SpreadDialogue(TutorialScript nowScript)
    {
        StopCoroutine(nowCoroutine);

        tutorialText.text = "";
        tutorialText.text = nowScript.tutorialScript;

        isScriptPrinting = false;
    }

    void SetAction(TutorialEvent getEvent)
    {
        switch(getEvent)
        {
            case TutorialEvent.Money:
                brightObjectMaskArray[0].SetActive(true);
                SetMask(true, 0);
                break;
            case TutorialEvent.MoneyEnd:
                brightObjectMaskArray[0].SetActive(false);
                SetMask(false, 0);
                break;
            case TutorialEvent.Recipe:
                brightObjectMaskArray[1].SetActive(true);
                SetMask(true, 1);
                break;
            case TutorialEvent.RecipeEnd:
                brightObjectMaskArray[1].SetActive(false);
                SetMask(false, 1);
                break;
            case TutorialEvent.Shelf:
                brightObjectMaskArray[2].SetActive(true);
                SetMask(true, 2);
                break;
            case TutorialEvent.ShelfEnd:
                brightObjectMaskArray[2].SetActive(false);
                SetMask(false, 2);
                break;
            case TutorialEvent.Cup:
                brightObjectMaskArray[3].SetActive(true);
                SetMask(true, 3);
                break;
            case TutorialEvent.CupEnd:
                brightObjectMaskArray[3].SetActive(false);
                SetMask(false, 3);
                break;
            case TutorialEvent.Night:
                dayBg.SetActive(false);
                nightBg.SetActive(true);
                break;
            case TutorialEvent.NightEnd:
                dayBg.SetActive(true);
                nightBg.SetActive(false);
                break;
            case TutorialEvent.TutorialEnd:
                pauseBtn.transform.SetParent(canvasTransform);
                pauseBtn.transform.SetSiblingIndex(9);
                tutorialTextParent.transform.SetParent(canvasTransform);
                tutorialTextParent.transform.SetSiblingIndex(10);
                StartCoroutine(EndTutorial());
                break;
        }
    }

    IEnumerator EndTutorial()
    {
        fade.TutorialFadeout(0.4f);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("DayScene");
    }


    void SetBrightObjectSlbling(GameObject nowObject)
    {
        nowObject.transform.SetParent(brightObjectsTransform);
    }

    //����ũ�� ����ϸ� isActive�� ���� �ȴ�.
    void SetMask(bool isActive, int nowMask)
    {
        if(isActive)
        {
            fadeImageTransform.SetParent(brightObjectMaskArray[nowMask].transform);
        }
        else
        {
            fadeImageTransform.SetParent(canvasTransform);
            fadeImageTransform.SetSiblingIndex(18);
        }
    }
}
