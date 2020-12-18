using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NativeWebSocket;


[Serializable]
public class Info
{
    public string info;
}

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

[Serializable]
public class Message
{
    public string user_name;
    public string message;
}

public class WebSocketController : MonoBehaviour
{
    private string WEBSOCKET_ADDRESS = "ws://untact-museum.herokuapp.com/";

    WebSocket websocket;
    public InviteListController inviteListController;
    public VideoController videoController;
    public TextController textController;
    public VideoApp videoApp;

    async void Start()
    {
        Dictionary<String, String> headers = new Dictionary<string, string>();
        headers.Add("Cookie", PlayerPrefs.GetString("Cookie"));
        websocket = new WebSocket(WEBSOCKET_ADDRESS + "ws/message/", headers);

        websocket.OnOpen += () =>
        {
            Debug.Log("WebSocket Connection open");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("WebSocket Connection error");
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket Connection close");
            PlayerPrefs.DeleteKey("Cookie");
            videoApp.unloadEngine();
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("LoginScene");
        };

        websocket.OnMessage += (bytes) =>
        {
            // Reading a plain text message
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            //Debug.Log(message);

            switch (OnMessageType(message))
            {
                case "leave_success":
                    //call textController.print() in VideoApp
                    break;
                case "leave_fail":
                    textController.print("이미 대화방에서 나온 상태입니다.");
                    break;
                case "username_self_fail":
                    textController.print("자신을 대상으로 요청을 보낼 수 없습니다.");
                    break;
                case "invite_success":
                    Info infoInviteSuccess = JsonUtility.FromJson<Info>(message); // username who you invited
                    textController.print(infoInviteSuccess.info + " 님을 초대했습니다.");
                    break;
                case "invite_same_channel_fail":
                    Info infoInviteSameChannelFail = JsonUtility.FromJson<Info>(message); // username who you invited
                    textController.print(infoInviteSameChannelFail.info + " 님과 이미 같은 대화방에 있습니다.");
                    break;
                case "accept_success":
                    Info infoAcceptSuccess = JsonUtility.FromJson<Info>(message); // username who you invited
                    textController.print(infoAcceptSuccess.info + " 님의 초대를 수락했습니다.");
                    break;
                case "accept_fail":
                    Info infoAcceptFail = JsonUtility.FromJson<Info>(message); // username who you invited
                    if (infoAcceptFail.info == "already_in_channel")
                    {
                        textController.print("초대를 수락하려면 대화방에서 나가야합니다.");
                    }
                    else if (infoAcceptFail.info == "channel_is_full")
                    {
                        textController.print("초대된 대화방의 정원이 7명으로 가득찼습니다.");
                    }
                    else // invitation_not_exist
                    {
                        textController.print("존재하지 않는 초대 건입니다.");
                    }
                    break;
                case "reject_success":
                    Info infoRejectSuccess = JsonUtility.FromJson<Info>(message); // username who you rejected
                    textController.print(infoRejectSuccess.info + " 님의 초대를 거절했습니다.");
                    break;
                case "reject_fail":
                    textController.print("존재하지 않는 초대 건입니다.");
                    break;
                case "cancel_success":
                    textController.print("초대를 취소했습니다.");
                    break;
                case "cancel_fail":
                    textController.print("존재하지 않는 초대 건입니다.");
                    break;
                case "type_fail":
                    textController.print("Request type is invalid.");
                    break;
                case "username_not_exist_fail":
                    textController.print("Cannot find any account associated with that username.");
                    break;
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
                    InvitationsState receivedInvitationsState = JsonUtility.FromJson<InvitationsState>(message);
                    inviteListController.ResetReceivedInvite();
                    foreach (Invitation invitation in receivedInvitationsState.invitations)
                    {
                        inviteListController.GetReceivedInvite(invitation);
                    }
                    inviteListController.ShowReceivedInvite();
                    break;
                case "channel_id_state":
                    ChannelIdState channelIdState = JsonUtility.FromJson<ChannelIdState>(message);
                    videoController.SetChannelID(channelIdState.channel_id);
                    break;
                case "message":
                    Message msg = JsonUtility.FromJson<Message>(message);
                    if (msg.message == "invited_you")
                    {
                        textController.print(msg.user_name + " 님이 당신을 초대했습니다.");
                    }
                    else if (msg.message == "accepted_you")
                    {
                        textController.print(msg.user_name + " 님이 당신의 초대를 수락했습니다.");
                    }
                    else if (msg.message == "rejected_you")
                    {
                        textController.print(msg.user_name + " 님이 당신의 초대를 거절했습니다.");
                    }
                    else if (msg.message == "canceled")
                    {
                        textController.print(msg.user_name + " 님이 당신에게 보낸 초대를 취소했습니다.");
                    }
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
