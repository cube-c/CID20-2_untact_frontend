using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserRow : MonoBehaviour
{
    public string userID;
    public string userTitle;

    public Text userIDText;
    public Text userTitleText;

    public Image buttonImage;
    public Button button;
    public Text buttonText;

    public WebSocketController webSocketController;

    void Awake()
    {
        webSocketController = GameObject.Find("WebSocketController").GetComponent<WebSocketController>();
    }

    public void FillText()
    {
        userIDText.text = userID;
        userTitleText.text = userTitle;
    }

    public void SetOnline()
    {
        buttonImage.color = new Color(50f / 255, 200f / 255, 50f / 255, 1);
        button.enabled = true;
        buttonText.text = "초대";
    }
    /*
    public void SetOffline()
    {
        buttonImage.color = new Color(200f / 255, 50f / 255, 50f / 255, 1);
        button.enabled = false;
        buttonText.text = "";
    }
    */
    public void SetDND()
    {
        buttonImage.color = new Color(200f / 255, 200f / 255, 50f / 255, 1);
        button.enabled = false;
        buttonText.text = "";
    }

    public void Invite()
    {
        webSocketController.Invite(userID);
    }
}
