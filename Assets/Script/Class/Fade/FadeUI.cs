using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    public Image fadeImage;
    public Text fadeText;
    float time = 0f;
    float fadeTime = 1f;
    
    public void FadeIn(){
        StartCoroutine(FadeInImage());
    }
    public void FadeOut(){
        StartCoroutine(FadeOutImage());
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
        //yield return new WaitForSeconds(1f);
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

    /*private IEnumerator FadeInText(){
        fadeText.gameObject.SetActive(true);
        Color alpha = fadeText.color;
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

    private IEnumerator FadeOutText(){
        fadeText.gameObject.SetActive(true);
        Color alpha = fadeText.color;
        time = 0f;
        while(alpha.a < 1f){
            time += Time.deltaTime / fadeTime;
            alpha.a = Mathf.Lerp(0, 1, time);
            fadeText.color = alpha;
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
        yield return null;
    }*/
}
