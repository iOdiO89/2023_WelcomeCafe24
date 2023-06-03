using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    public Image fadeImage;
    public Text fadeText;
    float time = 0f;
    float fadeTime = 1f;
    float tutorialTime = 1.5f;
    
    public void FadeIn(){
        StartCoroutine(FadeInImage());
    }
    public void FadeOut(){
        StartCoroutine(FadeOutImage());
    }

    public void TutorialFadeout(float startAhpha)
    {
        StartCoroutine(TutorialFadeOutImage(startAhpha));
    }

    public void TutorialFadeIn(float endAlpha)
    {
        StartCoroutine(TutorialFadeInImage(endAlpha));
    }

    private IEnumerator FadeOutImage(){
        fadeImage.gameObject.SetActive(true);
        Color alpha = fadeImage.color;
        time = 0f;
        while(alpha.a < 1f){
            time += Time.deltaTime / fadeTime;
            alpha.a = Mathf.Lerp(0, 1, time);
            fadeImage.color = alpha;
            yield return null;
        }
        yield return null;
    }

    private IEnumerator TutorialFadeOutImage(float startAlpha)
    {
        fadeImage.gameObject.SetActive(true);
        Color alpha = fadeImage.color;
        time = 0f;
        while (alpha.a < 1f)
        {
            time += Time.deltaTime / tutorialTime;
            alpha.a = Mathf.Lerp(startAlpha, 1, time);
            fadeImage.color = alpha;
            yield return null;
        }
        yield return null;
    }

    private IEnumerator FadeInImage(){
        fadeImage.gameObject.SetActive(true);
        Color alpha = fadeImage.color;
        time = 0f;
        while(alpha.a > 0f){
            time += Time.deltaTime / fadeTime;
            alpha.a = Mathf.Lerp(1, 0, time);
            fadeImage.color = alpha;
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
        yield return null;
    }

    private IEnumerator TutorialFadeInImage(float endAlpha)
    {
        fadeImage.gameObject.SetActive(true);
        Color alpha = fadeImage.color;
        time = 0f;
        while (alpha.a > endAlpha)
        {
            time += Time.deltaTime / tutorialTime;
            alpha.a = Mathf.Lerp(1, endAlpha, time);
            fadeImage.color = alpha;
            yield return null;
        }

        fadeText.gameObject.SetActive(false);
        yield return null;
    }
}
