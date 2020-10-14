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
            ModelInfo modelInfo = hit.transform.GetComponent<ModelInfo>();
            if (modelInfo != null)
            {
                focus.text = "<color=blue>+</color>";
                if (Input.GetKeyDown(KeyCode.E))
                {
                    text.text = "<size=25><b>" + modelInfo.modelName + "</b></size>" + "\n" + "<size=15><b>" + modelInfo.modelSummary + "</b></size>" + "\n" + modelInfo.modelInfo;
                }
                return;
            }
        }
        focus.text = "+";
    }
}
