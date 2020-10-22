using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public List<GameObject> exhibitsAll;
    public List<GameObject> exhibitsShow;
    public int selectedIndex;

    SortOrder sortOrder = SortOrder.POSITION_ID_ASCENDING;

    void Awake()
    {
        exhibitsAll = new List<GameObject>();
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

        exhibitsAll.Add(exhibitRow);
    }
}
