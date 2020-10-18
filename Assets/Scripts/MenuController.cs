using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject menu;
    public GameObject menuBackground;
    public bool menuOn = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuOn = !menuOn;
            if (menuOn)
            {
                gameObject.GetComponent<FirstPersonMovement>().enabled = false;
                gameObject.GetComponent<Jump>().enabled = false;
                gameObject.GetComponent<Crouch>().enabled = false;
                gameObject.GetComponentInChildren<FirstPersonLook>().enabled = false;
                gameObject.GetComponentInChildren<Zoom>().enabled = false;
                menu.SetActive(true);
                menuBackground.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                gameObject.GetComponent<FirstPersonMovement>().enabled = true;
                gameObject.GetComponent<Jump>().enabled = true;
                gameObject.GetComponent<Crouch>().enabled = true;
                gameObject.GetComponentInChildren<FirstPersonLook>().enabled = true;
                gameObject.GetComponentInChildren<Zoom>().enabled = true;
                menu.SetActive(false);
                menuBackground.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
