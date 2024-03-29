using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BouncyUI : MonoBehaviour
{   
    Vector3[] originalPos;
    float speed = 5.0f;
    [SerializeField] Text[] innerText;

    void Start(){ 
        originalPos = new Vector3[3];  
        for(int i=0; i<3; i++){
            originalPos[i] = innerText[i].transform.localPosition;
        }
    }

    public void Up(int index){
        StartCoroutine("UPBounce", index);
        innerText[index].transform.localPosition = originalPos[index];
    }

    public void Down(){
        StartCoroutine("DownBounce");
        innerText[2].transform.localPosition = originalPos[2];
    }

    IEnumerator UPBounce(int index){
        innerText[index].gameObject.SetActive(true);
        float fadeCount = 1.0f;
        while(fadeCount > 0.0f){
            fadeCount -= 0.01f; // 값이 작을수록 천천히 fadeout
            innerText[index].transform.localPosition += new Vector3(0, 5, 0) * speed * Time.deltaTime;
            yield return null;
            innerText[index].color = new Color(94/255f, 50/255f, 6/255f, fadeCount);
        }
        innerText[index].gameObject.SetActive(false);
    }

    IEnumerator DownBounce(){
        innerText[2].gameObject.SetActive(true);
        float fadeCount = 1.0f;
        while(fadeCount > 0.0f){
            fadeCount -= 0.01f;
            innerText[2].transform.localPosition += new Vector3(0, -5, 0) * speed * Time.deltaTime;
            yield return null;
            innerText[2].color = new Color(94/255f, 50/255f, 6/255f, fadeCount);
        }
        innerText[2].gameObject.SetActive(false);
    }
}