using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickUserTitle : MonoBehaviour, IPointerDownHandler
{
    public UserListController userListController;

    public void OnPointerDown(PointerEventData eventData)
    {
        userListController.ClickTitle();
    }
}