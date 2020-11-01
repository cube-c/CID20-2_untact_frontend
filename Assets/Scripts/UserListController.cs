using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


[Serializable]
public class User
{
    public string name;
    public string title;
    public string status;
}

[Serializable]
public class UserList
{
    public User[] users;
}

enum UserSortOrder
{
    USER_ID_ASCENDING,
    USER_ID_DESCENDING,
    USER_TITLE_ASCENDING,
    USER_TITLE_DESCENDING
};

public class UserListController : MonoBehaviour
{
    public GameObject content;
    public Scrollbar scrollbar;
    public Image handleImage;

    public GameObject userRowPrefab;

    public const float rowHeight = 40;
    public const int numberOfRowsInWindow = 10;

    public Text textID;
    public Text textTitle;
    public Text searchText;
    public Dropdown searchBy;

    public List<GameObject> userRowsAll;
    public List<GameObject> userRowsShow;


    UserSortOrder sortOrder = UserSortOrder.USER_ID_ASCENDING;

    void Awake()
    {
        userRowsAll = new List<GameObject>();
    }

    public void GetUser(User user)
    {
        GameObject userRow = Instantiate(userRowPrefab);
        UserRow userRowComponent = userRow.GetComponent<UserRow>();

        userRowComponent.userID = user.name;
        userRowComponent.userTitle = user.title;
        userRowComponent.FillText();
        switch (user.status)
        {
            case "online":
                userRowComponent.SetOnline();
                break;
            /*
            case "offline":
                userRowComponent.SetOffline();
                break;
            */
            case "dnd":
                userRowComponent.SetDND();
                break;
            default:
                break;
        }

        userRowsAll.Add(userRow);
    }

    public void Refresh() // runs when menu button or refresh button clicked
    {
        foreach (GameObject userRow in userRowsAll)
        {
            Destroy(userRow);
        }
        userRowsAll = new List<GameObject>();
        StartCoroutine(GetUserRequest());
    }

    IEnumerator GetUserRequest()
    {
        using (UnityWebRequest req = UnityWebRequest.Get("http://localhost:8000/api/userStatus/"))
        {
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error);
            }
            else
            {
                UserList userList = JsonUtility.FromJson<UserList>("{\"users\":" + req.downloadHandler.text + "}");
                foreach (User user in userList.users)
                {
                    GetUser(user);
                }
            }
        }
        SetShowAll();
        Show();
    }

    public void SetShowAll()
    {
        userRowsShow = new List<GameObject>(userRowsAll);
        Sort();
    }

    public void ClickID()
    {
        if (sortOrder == UserSortOrder.USER_ID_ASCENDING)
        {
            sortOrder = UserSortOrder.USER_ID_DESCENDING;
            textID.text = "ID <size=12>▲</size>";
            textTitle.text = "직책";
        }
        else
        {
            sortOrder = UserSortOrder.USER_ID_ASCENDING;
            textID.text = "ID <size=12>▼</size>";
            textTitle.text = "직책";
        }

        Sort();
        Show();
    }

    public void ClickTitle()
    {
        if (sortOrder == UserSortOrder.USER_TITLE_ASCENDING)
        {
            sortOrder = UserSortOrder.USER_TITLE_DESCENDING;
            textID.text = "ID";
            textTitle.text = "직책 <size=12>▲</size>";
        }
        else
        {
            sortOrder = UserSortOrder.USER_TITLE_ASCENDING;
            textID.text = "ID";
            textTitle.text = "직책 <size=12>▼</size>";
        }

        Sort();
        Show();
    }

    public void Sort()
    {
        switch (sortOrder)
        {
            case UserSortOrder.USER_ID_ASCENDING:
                userRowsShow = userRowsShow.OrderBy(userRow => userRow.GetComponent<UserRow>().userID).ToList();
                break;
            case UserSortOrder.USER_ID_DESCENDING:
                userRowsShow = userRowsShow.OrderByDescending(userRow => userRow.GetComponent<UserRow>().userID).ToList();
                break;
            case UserSortOrder.USER_TITLE_ASCENDING:
                userRowsShow = userRowsShow.OrderBy(userRow => userRow.GetComponent<UserRow>().userTitle).ToList();
                break;
            case UserSortOrder.USER_TITLE_DESCENDING:
                userRowsShow = userRowsShow.OrderByDescending(userRow => userRow.GetComponent<UserRow>().userTitle).ToList();
                break;
            default:
                break;
        }
    }

    public void Search()
    {
        if (searchText.text != "")
        {
            foreach (GameObject userRow in userRowsShow)
            {
                userRow.SetActive(false);
            }

            if (searchBy.value == 0) // search by ID
            {
                userRowsShow = userRowsAll.Where(userRow => userRow.GetComponent<UserRow>().userID.ToUpper().Contains(searchText.text.ToUpper())).ToList();
            }
            else // search by Title
            {
                userRowsShow = userRowsAll.Where(userRow => userRow.GetComponent<UserRow>().userTitle.ToUpper().Contains(searchText.text.ToUpper())).ToList();
            }

            Sort();
            Show();
        }
        else
        {
            SetShowAll();
            Show();
        }
    }

    public void Show()
    {
        RectTransform rt = content.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, userRowsShow.Count * rowHeight);

        scrollbar.numberOfSteps = userRowsShow.Count + 1 - numberOfRowsInWindow;
        if (scrollbar.numberOfSteps <= 1)
        {
            handleImage.color = new Color(1, 1, 1, 0);
        }
        else
        {
            handleImage.color = new Color(1, 1, 1, 200f / 255);
        }

        float posy = 0;
        foreach (GameObject userRow in userRowsShow)
        {
            userRow.SetActive(true);
            userRow.transform.SetParent(content.transform);
            userRow.GetComponent<RectTransform>().localPosition = new Vector3(0, posy, 0);
            posy -= rowHeight;
        }
    }
}
