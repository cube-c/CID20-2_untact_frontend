using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using agora_gaming_rtc;
using agora_utilities;

[Serializable]
public class UserVideoData
{
    public uint uid;
    public string name;
    public string title;
}

public class VideoApp : MonoBehaviour
{
    private string SITE_ADDRESS = "http://untact-museum.herokuapp.com/";
    private int userVideoPanelWidth = 240;
    private int userVideoPanelHeight = 200;

    public MyInfo myInfo;
    private IRtcEngine engine;
    public GameObject userVideoPrefab;
    public List<GameObject> userVideoList;
    public GameObject myVideo;
    public VideoSurface myVideoSurface;
    public Transform videoPanelTransform;

    private bool myVideoIsOn = false;

    public void loadEngine(string appId)
    {
        Debug.Log("calling loadEngine");

        if (engine != null)
        {
            Debug.Log("Engine exists. Please unload it first!");
            return;
        }

        userVideoList = new List<GameObject>();
        engine = IRtcEngine.GetEngine(appId);

        VideoEncoderConfiguration config = new VideoEncoderConfiguration();
        config.dimensions.width = 320;
        config.dimensions.height = 180;

        engine.SetVideoEncoderConfiguration(config);

        engine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }

    private void activeMyVideo(bool activeVideo)
    {
        myVideoIsOn = activeVideo;
        myVideoSurface.SetEnable(activeVideo);
        myVideo.SetActive(activeVideo);
    }

    public void unloadEngine()
    {
        Debug.Log("calling unloadEngine");

        if (engine != null)
        {
            IRtcEngine.Destroy();
            engine = null;
        }
    }

    public void join(string channelId)
    {
        Debug.Log("calling join");

        if (engine == null) return;

        engine.OnJoinChannelSuccess = onJoinChannelSuccess;
        engine.OnUserJoined = onUserJoined;
        engine.OnUserOffline = onUserOffline;

        engine.EnableVideo();
        engine.EnableVideoObserver();
        engine.JoinChannel(channelId, null, (uint)myInfo.uid);
        activeMyVideo(true);
    }

    public void leave()
    {
        Debug.Log("calling leave");

        if (engine == null) return;

        foreach (GameObject userVideoObject in userVideoList)
        {
            Destroy(userVideoObject);
        }
        userVideoList = new List<GameObject>();
        engine.LeaveChannel();
        engine.DisableVideoObserver();
        activeMyVideo(false);
    }

    public void EnableVideo(bool pauseVideo)
    {
        if (engine != null)
        {
            if(!pauseVideo)
            {
                engine.EnableVideo();
            }
            else
            {
                engine.DisableVideo();
            }
        }
    }

    private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        // print join success
    }

    private void onUserJoined(uint uid, int elapsed)
    {
        StartCoroutine(GetUserVideo(uid));
    }

    private void onUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // print user leaved
        GameObject offlineUserVideoObject = userVideoList.Where(userVideo => userVideo.GetComponent<UserVideo>().uid == uid).SingleOrDefault();
        userVideoList.Remove(offlineUserVideoObject);
        Destroy(offlineUserVideoObject);
        refreshUserVideo();
    }

    IEnumerator GetUserVideo(uint uid)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(SITE_ADDRESS + "api/userStatus/" + uid.ToString()))
        {
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error);
            }
            else
            {
                GameObject userVideoObject = Instantiate(userVideoPrefab);
                UserVideoData userVideoData = JsonUtility.FromJson<UserVideoData>(req.downloadHandler.text);
                UserVideo userVideoComponent = userVideoObject.GetComponent<UserVideo>();
                userVideoComponent.uid = userVideoData.uid;
                userVideoComponent.username = userVideoData.name;
                userVideoComponent.title = userVideoData.title;
                userVideoComponent.FillText();

                VideoSurface userVideoSurface = userVideoObject.transform.Find("VideoScreen").gameObject.AddComponent<VideoSurface>();
                userVideoSurface.SetForUser(uid);
                userVideoSurface.SetEnable(true);
                userVideoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                userVideoSurface.SetGameFps(30);

                userVideoList.Add(userVideoObject);
                refreshUserVideo();
            }
        }
    }

    public void refreshUserVideo()
    {
        foreach (GameObject userVideoObject in userVideoList)
        {
            int posx = (userVideoList.IndexOf(userVideoObject) / 3) * (-userVideoPanelWidth);
            int posy = (userVideoList.IndexOf(userVideoObject) % 3) * (-userVideoPanelHeight);
            userVideoObject.transform.SetParent(videoPanelTransform);
            userVideoObject.GetComponent<RectTransform>().localPosition = new Vector3(posx, posy, 0);
        }
    }
}
