using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExhibitRow : MonoBehaviour
{
    public string positionId;
    public string exhibitName;
    public string summary;
    public float posx;
    public float posy;
    public float posz;
    public float roty;

    public Text positionIdText;
    public Text exhibitNameText;
    public Image selectedRowImage;

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
