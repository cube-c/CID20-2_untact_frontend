using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using agora_gaming_rtc;

public class UserVideo : MonoBehaviour
{
    public Text nameText;
    public Text titleText;

    public uint uid;
    public string username;
    public string title;

    public void FillText()
    {
        nameText.text = username;
        titleText.text = title;
    }
}
