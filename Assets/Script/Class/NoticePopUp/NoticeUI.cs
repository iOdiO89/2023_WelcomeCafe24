using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NoticeUI : MonoBehaviour
{
    [Header("SubNotice")]
    public GameObject subbox;
    public Text subintext;
    public Animator subani;

    private WaitForSeconds _UIDelay1 = new WaitForSeconds(1.5f);
    private WaitForSeconds _UIDelay2 = new WaitForSeconds(0.3f);

    void Start(){
        subbox.SetActive(false);
    }

    public void SUB(string msg){
        subintext.text = msg;
        subbox.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(SUBDelay());
    }

    IEnumerator SUBDelay(){
        subbox.SetActive(true);
        subani.SetBool("isOn", true);
        yield return _UIDelay1;
        subani.SetBool("isOn", false);
        yield return _UIDelay2;
        subbox.SetActive(false);
    }
}
