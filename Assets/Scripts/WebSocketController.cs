using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NativeWebSocket;

[Serializable]
public class InvitationsState
{
    public string type;
    public Invitation[] invitations;
}

[Serializable]
public class ChannelIdState
{
    public string channel_id;
}

public class WebSocketController : MonoBehaviour
{
    private string WEBSOCKET_ADDRESS = "ws://untact-museum.herokuapp.com/";

    WebSocket websocket;
    public InviteListController inviteListController;
    public VideoController videoController;

    async void Start()
    {
        Dictionary<String, String> headers = new Dictionary<string, string>();
        headers.Add("Cookie", PlayerPrefs.GetString("Cookie"));
        websocket = new WebSocket(WEBSOCKET_ADDRESS + "ws/message/", headers);

        websocket.OnOpen += () =>
        {
            //Debug.Log("WebSocket Connection open");
        };

        websocket.OnError += (e) =>
        {
            //Debug.Log("Error - " + e);
        };

        websocket.OnClose += (e) =>
        {
            //Debug.Log("WebSocket Connection closed");
        };

        websocket.OnMessage += (bytes) =>
        {
            // Reading a plain text message
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log(message);

            switch (OnMessageType(message))
            {
                case "sent_invitations_state":
                    InvitationsState sentInvitationsState = JsonUtility.FromJson<InvitationsState>(message);
                    inviteListController.ResetSentInvite();
                    foreach (Invitation invitation in sentInvitationsState.invitations)
                    {
                        inviteListController.GetSentInvite(invitation);
                    }
                    inviteListController.ShowSentInvite();
                    break;
                case "received_invitations_state":
                    Debug.Log("received_invitations_state");
                    InvitationsState receivedInvitationsState = JsonUtility.FromJson<InvitationsState>(message);
                    inviteListController.ResetReceivedInvite();
                    foreach (Invitation invitation in receivedInvitationsState.invitations)
                    {
                        inviteListController.GetReceivedInvite(invitation);
                    }
                    inviteListController.ShowReceivedInvite();
                    break;
                case "channel_id_state":
                    Debug.Log("channel_id_state");
                    ChannelIdState channelIdState = JsonUtility.FromJson<ChannelIdState>(message);
                    videoController.SetChannelID(channelIdState.channel_id);
                    break;
                default:
                    break;
            }
        };

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    public string OnMessageType(string jsonMessage)
    {
        string typeString = jsonMessage.Split(',')[0];
        char[] trimChars = { ' ', '\"' };
        return typeString.Split(':')[1].Trim(trimChars);
    }

    async public void Invite(string userID)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("{\"type\":\"invite\",\"username\":\"" + userID + "\"}");
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

    async public void Cancel(string userID)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("{\"type\":\"cancel\",\"username\":\"" + userID + "\"}");
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
