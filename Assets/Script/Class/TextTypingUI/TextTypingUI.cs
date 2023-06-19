using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTypingUI : MonoBehaviour
{
    public Text text;
    private string msg;
    private float speed = 0.2f;

    public void Typing(string msg){
        StartCoroutine(Type(msg));
    }
    
    IEnumerator Type(string msg){
        for(int i=0; i<msg.Length; i++){
            text.text = msg.Substring(0, i+1);
            yield return new WaitForSeconds(speed);
        }
    }
}
