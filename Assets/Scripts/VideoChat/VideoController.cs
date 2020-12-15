using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using agora_gaming_rtc;
using agora_utilities;
public class VideoController : MonoBehaviour
{
    private string APP_ID = "e2ac63508931414ab0423712cf4b6170";

    static VideoApp app = null;

    public GameObject videoObject;

    private void Start()
    {
        if(ReferenceEquals(app,null))
        {
            app = new VideoApp();
            app.videoObject = this.videoObject;
            app.loadEngine(APP_ID);
        }
    }

    public void SetChannelID(string channelId)
    {
        if (string.IsNullOrEmpty(channelId))
        {
            Debug.Log("leave");
        }
        else
        {
            app.join(channelId);
            Debug.Log("join");
        }
    }
}
