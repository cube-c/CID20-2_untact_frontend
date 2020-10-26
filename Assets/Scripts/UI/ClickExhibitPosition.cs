using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickExhibitPosition : MonoBehaviour, IPointerDownHandler
{
    public ExhibitListController exhibitListController;

    public void OnPointerDown(PointerEventData eventData)
    {
        exhibitListController.ClickPosition();
    }
}
