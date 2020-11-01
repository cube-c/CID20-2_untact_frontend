using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExhibitRow : MonoBehaviour
{
    public string positionId;
    public string exhibitName;
    public string summary;
    public Vector3 teleportPosition;
    public float teleportRoty;

    public Text positionIdText;
    public Text exhibitNameText;
    public Image selectedRowImage;

    public GameObject firstPersonController;


    void Awake()
    {
        firstPersonController = GameObject.Find("FirstPersonController");
    }

    public void FillText()
    {
        positionIdText.text = positionId;
        exhibitNameText.text = exhibitName;
    }

    public void Mark(bool isSelected)
    {
        if (isSelected)
        {
            selectedRowImage.color = new Color(0, 0, 0, 100f / 255);
        }
        else
        {
            selectedRowImage.color = new Color(0, 0, 0, 0);
        }
    }

    public void Teleport()
    {
        firstPersonController.transform.position = teleportPosition;

        Vector3 teleportTransformEulerAngle = new Vector3(0, teleportRoty, 0);
        firstPersonController.transform.eulerAngles = teleportTransformEulerAngle;

        Vector2 teleportLook = new Vector2(teleportRoty, 0);
        firstPersonController.GetComponentInChildren<FirstPersonLook>().SetLook(teleportLook);

        firstPersonController.transform.Find("Main Camera").transform.localEulerAngles = Vector3.zero;
    }
}
