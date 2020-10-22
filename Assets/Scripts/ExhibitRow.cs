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

    public void FillText()
    {
        positionIdText.text = positionId;
        exhibitNameText.text = exhibitName;
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
