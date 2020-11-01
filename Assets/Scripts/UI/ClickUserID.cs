using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickUserID : MonoBehaviour, IPointerDownHandler
{
    public UserListController userListController;

    public void OnPointerDown(PointerEventData eventData)
    {
        userListController.ClickID();
    }
}