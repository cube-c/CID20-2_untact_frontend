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

    public GameObject exhibitRowPrefab;

    public const float rowHeight = 25;
    public const int numberOfRowsInWindow = 11;

    public Text textID;
    public Text textTitle;
    public Text searchText;

    public List<GameObject> userRowsAll;
    public List<GameObject> userRowsShow;
    public GameObject userRowSummary;

    UserSortOrder sortOrder = UserSortOrder.USER_ID_ASCENDING;

    void Awake()
    {
        userRowsAll = new List<GameObject>();
    }

    public void Refresh()
    {

    }

    public void ClickID()
    {

    }

    public void ClickTitle()
    {

    }

    public void Sort()
    {

    }

    public void Search()
    {

    }

    public void Show()
    {

    }
}
