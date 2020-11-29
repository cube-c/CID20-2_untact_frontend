using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NativeWebSocket;

public class WebSocketController : MonoBehaviour
{
    WebSocket websocket;
    public Text debugText;

    async void Start()
    {
        Dictionary<String, String> headers = new Dictionary<string, string>();
        Debug.Log(PlayerPrefs.GetString("Cookie"));
        headers.Add("Cookie", PlayerPrefs.GetString("Cookie"));
        websocket = new WebSocket("ws://localhost:8000/ws/message/", headers);

        websocket.OnOpen += () =>
        {
            Debug.Log("WebSocket Connection open");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error - " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket Connection closed");
        };

        websocket.OnMessage += (bytes) =>
        {
            // Reading a plain text message
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received OnMessage (" + bytes.Length + " bytes) " + message);
            debugText.text += message + "\n";
        };

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    async public void Invite(string userID)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("{\"type\":\"invite\",\"username\":\""+userID+"\"}");
        }
    }

    async public void Accept(string userID)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("{\"type\":\"accept\",\"username\":\"" + userID + "\"}");
        }
    }

    async public void Reject(string userID)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("{\"type\":\"reject\",\"username\":\"" + userID + "\"}");
        }
    }

    async public void Leave()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("{\"type\":\"leave\"}");
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}
