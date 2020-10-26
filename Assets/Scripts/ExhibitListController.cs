using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum SortOrder
{
    POSITION_ID_ASCENDING,
    POSITION_ID_DESCENDING,
    EXHIBIT_NAME_ASCENDING,
    EXHIBIT_NAME_DESCENDING
};

public class ExhibitListController : MonoBehaviour
{
    public GameObject content;
    public Scrollbar scrollbar;
    public Image handleImage;

    public GameObject exhibitRowPrefab;

    public const float rowHeight = 25;
    public const int numberOfRowsInWindow = 11;

    public Text textPosition;
    public Text textName;

    public List<GameObject> exhibitRowsAll;
    public List<GameObject> exhibitRowsShow;
    public GameObject exhibitRowSummary;
    public GameObject summaryRow;

    SortOrder sortOrder = SortOrder.POSITION_ID_ASCENDING;

    void Awake()
    {
        exhibitRowsAll = new List<GameObject>();
    }

    public void GetExhibit(GameObject exhibit)
    {
        ExhibitData exhibitData = exhibit.GetComponent<ExhibitData>();

        GameObject exhibitRow = Instantiate(exhibitRowPrefab);
        ExhibitRow exhibitRowComponent = exhibitRow.GetComponent<ExhibitRow>();

        exhibitRowComponent.positionId = exhibitData.positionId;
        exhibitRowComponent.exhibitName = exhibit.name;
        exhibitRowComponent.summary = exhibitData.summary;
        exhibitRowComponent.posx = exhibit.transform.position.x;
        exhibitRowComponent.posy = exhibit.transform.position.y;
        exhibitRowComponent.posz = exhibit.transform.position.z;
        exhibitRowComponent.roty = exhibit.transform.rotation.eulerAngles.y;
        exhibitRowComponent.FillText();

        exhibitRowsAll.Add(exhibitRow);
    }

    public void SetShowAll()
    {
        exhibitRowsShow = new List<GameObject>(exhibitRowsAll);
        Sort();
    }

    public void ClickPosition()
    {
        if (sortOrder == SortOrder.POSITION_ID_ASCENDING)
        {
            sortOrder = SortOrder.POSITION_ID_DESCENDING;
            textPosition.text = "위치 <size=8>▲</size>";
            textName.text = "이름";
        }
        else
        {
            sortOrder = SortOrder.POSITION_ID_ASCENDING;
            textPosition.text = "위치 <size=8>▼</size>";
            textName.text = "이름";
        }

        Sort();
        Show();
    }

    public void ClickName()
    {
        if (sortOrder == SortOrder.EXHIBIT_NAME_ASCENDING)
        {
            sortOrder = SortOrder.EXHIBIT_NAME_DESCENDING;
            textPosition.text = "위치";
            textName.text = "이름 <size=8>▲</size>";
        }
        else
        {
            sortOrder = SortOrder.EXHIBIT_NAME_ASCENDING;
            textPosition.text = "위치";
            textName.text = "이름 <size=8>▼</size>";
        }

        Sort();
        Show();
    }

    public void Sort()
    {
        switch (sortOrder)
        {
            case SortOrder.POSITION_ID_ASCENDING:
                exhibitRowsShow = exhibitRowsShow.OrderBy(exhibitRow => exhibitRow.GetComponent<ExhibitRow>().positionId).ToList();
                break;
            case SortOrder.POSITION_ID_DESCENDING:
                exhibitRowsShow = exhibitRowsShow.OrderByDescending(exhibitRow => exhibitRow.GetComponent<ExhibitRow>().positionId).ToList();
                break;
            case SortOrder.EXHIBIT_NAME_ASCENDING:
                exhibitRowsShow = exhibitRowsShow.OrderBy(exhibitRow => exhibitRow.GetComponent<ExhibitRow>().exhibitName).ToList();
                break;
            case SortOrder.EXHIBIT_NAME_DESCENDING:
                exhibitRowsShow = exhibitRowsShow.OrderByDescending(exhibitRow => exhibitRow.GetComponent<ExhibitRow>().exhibitName).ToList();
                break;
            default:
                break;
        }
    }

    public void SwitchSummary(GameObject exhibitRow)
    {
        if (exhibitRow != exhibitRowSummary)
        {
            if (exhibitRowSummary != null)
            {
                exhibitRowSummary.GetComponent<ExhibitRow>().Mark(false);
            }
            exhibitRowSummary = exhibitRow;
            exhibitRowSummary.GetComponent<ExhibitRow>().Mark(true);
            summaryRow.GetComponentInChildren<TMP_Text>().text = exhibitRow.GetComponent<ExhibitRow>().summary;
            summaryRow.SetActive(true);

            gameObject.GetComponent<SummaryScroller>().SetTextChanged(true);
        }
        else
        {
            summaryRow.SetActive(!summaryRow.activeSelf);
            exhibitRowSummary.GetComponent<ExhibitRow>().Mark(summaryRow.activeSelf);
            gameObject.GetComponent<SummaryScroller>().SetTextChanged(summaryRow.activeSelf);
        }

        Show();
    }

    public void Show()
    {
        RectTransform rt = content.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (exhibitRowsShow.Count + 1) * rowHeight);

        scrollbar.numberOfSteps = exhibitRowsShow.Count + 2 - numberOfRowsInWindow;
        if (scrollbar.numberOfSteps <= 1)
        {
            handleImage.color = new Color(1, 1, 1, 0);
        }
        else
        {
            handleImage.color = new Color(1, 1, 1, 200f / 255);
        }

        float posy = 0;
        if (!summaryRow.activeSelf)
        {
            foreach (GameObject exhibitRow in exhibitRowsShow)
            {
                exhibitRow.transform.SetParent(content.transform);
                exhibitRow.GetComponent<RectTransform>().localPosition = new Vector3(0, posy, 0);
                posy -= rowHeight;
            }
        }
        else
        {
            foreach (GameObject exhibitRow in exhibitRowsShow)
            {
                exhibitRow.transform.SetParent(content.transform);
                exhibitRow.GetComponent<RectTransform>().localPosition = new Vector3(0, posy, 0);
                posy -= rowHeight;

                if (exhibitRow == exhibitRowSummary)
                {
                    summaryRow.GetComponent<RectTransform>().localPosition = new Vector3(0, posy, 0);
                    posy -= rowHeight;
                }
            }
        }
    }
}
