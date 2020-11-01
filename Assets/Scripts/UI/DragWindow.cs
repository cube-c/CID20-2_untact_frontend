using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public RectTransform windowPanelTransform;
    public Image otherWindowBar;

    public void OnDrag(PointerEventData eventData)
    {
        GetComponent<Image>().color = new Color(150f / 255, 150f / 255, 150f / 255, 100f / 255);
        windowPanelTransform.anchoredPosition += eventData.delta;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GetComponent<Image>().color = new Color(150f / 255, 150f / 255, 150f / 255, 100f / 255);
        windowPanelTransform.SetAsLastSibling();
        otherWindowBar.color = new Color(200f / 255, 200f / 255, 200f / 255, 100f / 255);
    }
}
