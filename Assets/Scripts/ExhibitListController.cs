using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum ExhibitSortOrder
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
    public ScrollRect scrollRect;

    public GameObject exhibitRowPrefab;

    public const float rowHeight = 40;
    public const int numberOfRowsInWindow = 10;

    public Text textPosition;
    public Text textName;
    public Text searchText;

    public List<GameObject> exhibitRowsAll;
    public List<GameObject> exhibitRowsShow;
    public GameObject exhibitRowSummary;
    public GameObject summaryRow;

    ExhibitSortOrder sortOrder = ExhibitSortOrder.POSITION_ID_ASCENDING;

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
        exhibitRowComponent.teleportPosition = exhibit.transform.position + exhibit.transform.forward * 2.0f;
        exhibitRowComponent.teleportRoty = exhibit.transform.rotation.eulerAngles.y + 180.0f;
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
        if (sortOrder == ExhibitSortOrder.POSITION_ID_ASCENDING)
        {
            sortOrder = ExhibitSortOrder.POSITION_ID_DESCENDING;
            textPosition.text = "위치 <size=12>▲</size>";
            textName.text = "이름";
        }
        else
        {
            sortOrder = ExhibitSortOrder.POSITION_ID_ASCENDING;
            textPosition.text = "위치 <size=12>▼</size>";
            textName.text = "이름";
        }

        Sort();
        Show();
    }

    public void ClickName()
    {
        if (sortOrder == ExhibitSortOrder.EXHIBIT_NAME_ASCENDING)
        {
            sortOrder = ExhibitSortOrder.EXHIBIT_NAME_DESCENDING;
            textPosition.text = "위치";
            textName.text = "이름 <size=12>▲</size>";
        }
        else
        {
            sortOrder = ExhibitSortOrder.EXHIBIT_NAME_ASCENDING;
            textPosition.text = "위치";
            textName.text = "이름 <size=12>▼</size>";
        }

        Sort();
        Show();
    }

    public void Sort()
    {
        switch (sortOrder)
        {
            case ExhibitSortOrder.POSITION_ID_ASCENDING:
                exhibitRowsShow = exhibitRowsShow.OrderBy(exhibitRow => exhibitRow.GetComponent<ExhibitRow>().positionId).ToList();
                break;
            case ExhibitSortOrder.POSITION_ID_DESCENDING:
                exhibitRowsShow = exhibitRowsShow.OrderByDescending(exhibitRow => exhibitRow.GetComponent<ExhibitRow>().positionId).ToList();
                break;
            case ExhibitSortOrder.EXHIBIT_NAME_ASCENDING:
                exhibitRowsShow = exhibitRowsShow.OrderBy(exhibitRow => exhibitRow.GetComponent<ExhibitRow>().exhibitName).ToList();
                break;
            case ExhibitSortOrder.EXHIBIT_NAME_DESCENDING:
                exhibitRowsShow = exhibitRowsShow.OrderByDescending(exhibitRow => exhibitRow.GetComponent<ExhibitRow>().exhibitName).ToList();
                break;
            default:
                break;
        }
    }

    public void Search()
    {
        if (searchText.text != "")
        {
            foreach (GameObject exhibitRow in exhibitRowsShow)
            {
                exhibitRow.SetActive(false);
            }

            exhibitRowsShow = exhibitRowsAll.Where(exhibitRow => exhibitRow.GetComponent<ExhibitRow>().exhibitName.ToUpper().Contains(searchText.text.ToUpper())).ToList();
            Sort();
            Show();
        }
        else
        {
            SetShowAll();
            Show();
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
            scrollRect.vertical = false;
        }
        else
        {
            scrollRect.vertical = true;
        }

        float posy = 0;
        if (!summaryRow.activeSelf)
        {
            foreach (GameObject exhibitRow in exhibitRowsShow)
            {
                exhibitRow.SetActive(true);
                exhibitRow.transform.SetParent(content.transform);
                exhibitRow.GetComponent<RectTransform>().localPosition = new Vector3(0, posy, 0);
                posy -= rowHeight;
            }
        }
        else
        {
            bool summaryRowIncluded = false;
            foreach (GameObject exhibitRow in exhibitRowsShow)
            {
                exhibitRow.SetActive(true);
                exhibitRow.transform.SetParent(content.transform);
                exhibitRow.GetComponent<RectTransform>().localPosition = new Vector3(0, posy, 0);
                posy -= rowHeight;

                if (exhibitRow == exhibitRowSummary)
                {
                    summaryRow.GetComponent<RectTransform>().localPosition = new Vector3(0, posy, 0);
                    posy -= rowHeight;

                    summaryRowIncluded = true;
                }
            }

            if (!summaryRowIncluded)
            {
                summaryRow.SetActive(false);
                exhibitRowSummary.GetComponent<ExhibitRow>().Mark(false);
            }
        }
    }
}
