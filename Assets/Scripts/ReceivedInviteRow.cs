using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceivedInviteRow : MonoBehaviour
{
    public string userID;
    public string userTitle;

    public Text userIDText;
    public Text userTitleText;

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

    public void Accept()
    {
        webSocketController.Accept(userID);
    }

    public void Reject()
    {
        webSocketController.Reject(userID);
    }
}
