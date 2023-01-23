using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Field : MonoBehaviour
{
    public bool isSpriteExist  = false;
    public Image fieldImage;

    private void Start()
    {
        fieldImage = this.GetComponent<Image>();
    }
}
