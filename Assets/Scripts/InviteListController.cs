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
public class Invitation
{
    public string name;
    public string title;
    public int timestamp;
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

    public MenuController menuController;

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

    public void GetReceivedInvite(Invitation invitation)
    {
        GameObject receivedInviteRow = Instantiate(receivedInviteRowPrefab);
        ReceivedInviteRow receivedInviteRowComponent = receivedInviteRow.GetComponent<ReceivedInviteRow>();

        receivedInviteRowComponent.userID = invitation.name;
        receivedInviteRowComponent.userTitle = invitation.title;
        receivedInviteRowComponent.FillText();

        receivedInviteRows.Add(receivedInviteRow);
    }

    public void GetSentInvite(Invitation invitation)
    {
        GameObject sentInviteRow = Instantiate(sentInviteRowPrefab);
        SentInviteRow sentInviteRowComponent = sentInviteRow.GetComponent<SentInviteRow>();

        sentInviteRowComponent.userID = invitation.name;
        sentInviteRowComponent.userTitle = invitation.title;
        sentInviteRowComponent.FillText();

        sentInviteRows.Add(sentInviteRow);
    }

    public void ShowReceivedInvite()
    {
        RectTransform rt = receivedInviteContent.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, receivedInviteRows.Count * receivedInviteRowHeight);

        receivedInviteScrollbar.numberOfSteps = receivedInviteRows.Count + 1 - numberOfReceivedInviteRowsInWindow;
        if (menuController.menuOn)
        {
            if (receivedInviteScrollbar.numberOfSteps <= 1)
            {
                receivedInviteScrollRect.vertical = false;
            }
            else
            {
                receivedInviteScrollRect.vertical = true;
            }
        }

        float posy = 0;
        foreach (GameObject receivedInviteRow in receivedInviteRows)
        {
            receivedInviteRow.SetActive(true);
            receivedInviteRow.transform.SetParent(receivedInviteContent.transform);
            receivedInviteRow.GetComponent<RectTransform>().localPosition = new Vector3(0, posy, 0);
            posy -= receivedInviteRowHeight;
        }
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
