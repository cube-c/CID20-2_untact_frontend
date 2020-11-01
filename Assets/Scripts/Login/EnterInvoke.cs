using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnterInvoke : MonoBehaviour
{
    public Button button;

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject && Input.GetKeyDown(KeyCode.Return) && button.IsActive())
        {
            button.onClick.Invoke();
        }
    }
}
