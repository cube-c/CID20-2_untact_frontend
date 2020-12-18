using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoController : MonoBehaviour
{
    public VideoApp app;

    [SerializeField]
    private string APP_ID = "e2ac63508931414ab0423712cf4b6170";

    void Awake()
    {
        app.loadEngine(APP_ID);
    }
    public void SetChannelID(string channelId)
    {
        if (string.IsNullOrEmpty(channelId)) // leave
        {
            app.leave();
        }
        else // join
        {
            app.join(channelId);
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (!ReferenceEquals(app, null))
        {
            app.EnableVideo(pause);
        }
    }

    void OnApplicationQuit()
    {
        if (!ReferenceEquals(app, null))
        {
            app.unloadEngine();
        }
    }
}
