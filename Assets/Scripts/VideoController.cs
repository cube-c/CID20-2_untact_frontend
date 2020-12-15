using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoController : MonoBehaviour
{
    private string APP_ID = "e2ac63508931414ab0423712cf4b6170";

    public void SetChannelID(string channelId)
    {
        if (string.IsNullOrEmpty(channelId))
        {
            Debug.Log("leave");
        }
        else
        {
            Debug.Log("join");
        }
    }
}
