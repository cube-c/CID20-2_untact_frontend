using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhibitListController : MonoBehaviour
{
    public GameObject content;
    public List<GameObject> exhibitList;
    public int selectedIndex;

    void Awake()
    {
        exhibitList = new List<GameObject>();
    }

    void resizeContent()
    {
        RectTransform rt = content.GetComponent<RectTransform>();
        int count = exhibitList.Count;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (count + 1) * 25);
    }

    public void GetExhibit(GameObject exhibit)
    {
        exhibitList.Add(exhibit);
        resizeContent();
    }
}
