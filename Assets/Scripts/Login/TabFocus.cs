using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabFocus : MonoBehaviour
{
    public Selectable nextSelectable;
    private bool updateSelect;

    void Start()
    {
        updateSelect = false;
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject && Input.GetKeyDown(KeyCode.Tab) && nextSelectable != null)
        {
            updateSelect = true;
        }
    }
    void LateUpdate()
    {
        if (updateSelect)
        {
            updateSelect = false;
            nextSelectable.Select();
        }

    }
}