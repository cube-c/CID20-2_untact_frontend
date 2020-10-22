using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject exhibitRowPrefab;
    public GameObject summaryRowPrefab;
    public const int rowHeight = 25;

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
        summaryRow = Instantiate(summaryRowPrefab);
        summaryRow.SetActive(false);
        summaryRow.transform.parent = content.transform;
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
    }

    public void SwitchSummary(GameObject exhibitRow)
    {
        if (exhibitRow != exhibitRowSummary)
        {
            exhibitRowSummary = exhibitRow;
            summaryRow.GetComponentInChildren<Text>().text = exhibitRow.GetComponent<ExhibitRow>().summary;
            summaryRow.SetActive(true);
        }
        else
        {
            summaryRow.SetActive(!summaryRow.activeSelf);
        }

        Show();
    }

    public void Show()
    {
        RectTransform rt = content.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, exhibitRowsShow.Count * rowHeight);

        int posy = 0;

        if (!summaryRow.activeSelf)
        {
            foreach (GameObject exhibitRow in exhibitRowsShow)
            {
                exhibitRow.transform.parent = content.transform;
                exhibitRow.GetComponent<RectTransform>().localPosition = new Vector3(0, posy, 0);
                posy -= rowHeight;
            }
        }
        else
        {
            foreach (GameObject exhibitRow in exhibitRowsShow)
            {
                exhibitRow.transform.parent = content.transform;
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
