using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    public const float rowHeight = 25;
    public const int numberOfRowsInWindow = 11;

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

    public void GetUser(/*argument*/)
    {
        // argument -> userRow
        // instantiate userRow
        // userRowsAll.Add(userRow)
    }

    public void Refresh() // runs when menu button or refresh button clicked
    {
        // foreach (userRow in userRowsAll)
        // { userRowsAll.Remove(userRow); Destory(userRow); }
        // run GetUserRequest coroutine
        // get all user status by GetUser (online, offline (+DND))
        // make new userRowsAll
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
            textID.text = "ID <size=8>▲</size>";
            textTitle.text = "직책";
        }
        else
        {
            sortOrder = UserSortOrder.USER_ID_ASCENDING;
            textID.text = "ID <size=8>▼</size>";
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
            textTitle.text = "직책 <size=8>▲</size>";
        }
        else
        {
            sortOrder = UserSortOrder.USER_TITLE_ASCENDING;
            textID.text = "ID";
            textTitle.text = "직책 <size=8>▼</size>";
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
