using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverLine : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text text;
    private string content;

    void Start()
    {
        text = gameObject.GetComponentInChildren<Text>();
        content = text.text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.text = "<b>" + content + "</b>";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.text = content;
    }
}
