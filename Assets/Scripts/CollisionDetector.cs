using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionDetector : MonoBehaviour
{
    public float distance = 3;
    public Text text;
    public Text focus;

    void Update()
    {
        RaycastHit hit;
        // Does the ray intersect any objects
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distance))
        {
            ExhibitData data = hit.transform.GetComponent<ExhibitData>();
            if (data != null)
            {
                focus.text = "<color=red>+</color>";
                if (Input.GetKeyDown(KeyCode.E))
                {
                    text.text = "<size=25><b>" + data.name + "</b></size>" + "    전시 위치 : " + data.positionId + "\n" +
                                "<size=15><b>" + data.summary + "</b></size>" + "\n" +
                                data.info;
                }
                return;
            }
        }
        focus.text = "+";
    }
}
