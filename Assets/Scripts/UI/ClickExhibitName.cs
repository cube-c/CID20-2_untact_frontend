using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickExhibitName : MonoBehaviour, IPointerDownHandler
{
    public ExhibitListController exhibitListController;

    public void OnPointerDown(PointerEventData eventData)
    {
        exhibitListController.ClickName();
    }
}
