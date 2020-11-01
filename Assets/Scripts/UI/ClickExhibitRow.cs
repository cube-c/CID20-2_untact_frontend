using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickExhibitRow : MonoBehaviour, IPointerDownHandler
{
    public ExhibitListController exhibitListController;

    void Awake()
    {
        exhibitListController = GameObject.Find("ExhibitListController").GetComponent<ExhibitListController>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        exhibitListController.SwitchSummary(gameObject);
    }
}
