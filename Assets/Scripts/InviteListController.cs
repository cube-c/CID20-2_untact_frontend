using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.EventSystems;


[Serializable]
public class Invite
{
    public string name;
    public string title;
}

[Serializable]
public class InviteList
{
    public Invite[] invites;
}

public class InviteListController : MonoBehaviour
{
    public GameObject receivedInviteContent;
    public GameObject sentInviteContent;

    public Scrollbar receivedInviteScrollbar;
    public Scrollbar sentInviteScrollbar;

    public ScrollRect receivedInviteScrollRect;
    public ScrollRect sentInviteScrollRect;

    public GameObject receivedInviteRowPrefab;
    public GameObject sentInviteRowPrefab;

    public const float receivedInviteRowHeight = 125;
    public const int numberOfReceivedInviteRowsInWindow = 4;

    public const float sentInviteRowHeight = 40;
    public const int numberOfSentInviteRowsInWindow = 5;

    public List<GameObject> receivedInviteRows;
    public List<GameObject> sentInviteRows;

    void Awake()
    {
        receivedInviteRows = new List<GameObject>();
        sentInviteRows = new List<GameObject>();
    }

    public void ResetReceivedInvite()
    {
        foreach (GameObject receivedInviteRow in receivedInviteRows)
        {
            Destroy(receivedInviteRow);
        }
        receivedInviteRows = new List<GameObject>();
    }

    public void ResetSentInvite()
    {
        foreach (GameObject sentInviteRow in sentInviteRows)
        {
            Destroy(sentInviteRow);
        }
        sentInviteRows = new List<GameObject>();
    }

    public void GetReceivedInvite(Invite invite)
    {
        GameObject receivedInviteRow = Instantiate(receivedInviteRowPrefab);
        ReceivedInviteRow receivedInviteRowComponent = receivedInviteRow.GetComponent<ReceivedInviteRow>();

        receivedInviteRowComponent.userID = invite.name;
        receivedInviteRowComponent.userTitle = invite.title;
        receivedInviteRowComponent.FillText();

        receivedInviteRows.Add(receivedInviteRow);
    }

    public void GetSentInvite(Invite invite)
    {
        GameObject sentInviteRow = Instantiate(sentInviteRowPrefab);
        SentInviteRow sentInviteRowComponent = sentInviteRow.GetComponent<SentInviteRow>();

        sentInviteRowComponent.userID = invite.name;
        sentInviteRowComponent.userTitle = invite.title;
        sentInviteRowComponent.FillText();

        sentInviteRows.Add(sentInviteRow);
    }

    public void ShowReceivedInvite()
    {
        // 1. 이거도 채우기 2. WebSocket 메세지 type 분류하는거 만들기 3. inviteRow 생성 처리
    }

    public void ShowSentInvite()
    {
        RectTransform rt = sentInviteContent.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, sentInviteRows.Count * sentInviteRowHeight);

        sentInviteScrollbar.numberOfSteps = sentInviteRows.Count + 1 - numberOfSentInviteRowsInWindow;
        if (sentInviteScrollbar.numberOfSteps <= 1)
        {
            sentInviteScrollRect.vertical = false;
        }
        else
        {
            sentInviteScrollRect.vertical = true;
        }

        float posy = 0;
        foreach (GameObject sentInviteRow in sentInviteRows)
        {
            sentInviteRow.SetActive(true);
            sentInviteRow.transform.SetParent(sentInviteContent.transform);
            sentInviteRow.GetComponent<RectTransform>().localPosition = new Vector3(0, posy, 0);
            posy -= sentInviteRowHeight;
        }
    }
}
