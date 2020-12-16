using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using agora_gaming_rtc;
using agora_utilities;

public class VideoApp : MonoBehaviour
{
    // instance of agora engine
    public GameObject videoObject;
    public UserVideo currUserVideoInfo;
    private IRtcEngine mRtcEngine;
    private List<UserVideo> videoUserList;
    private List<GameObject> videoScreenList;
    private List<Vector3> videoPositionList = new List<Vector3>()
        { new Vector3(-75, -55), new Vector3(-215, -55), new Vector3(-75, -150), new Vector3(-215, -150), new Vector3(-75, -245), new Vector3(-215, -245), new Vector3(-75, -340)};
    // load agora engine
    public void loadEngine(string appId)
    {
        // start sdk
        Debug.Log("initializeEngine");

        if (mRtcEngine != null)
        {
            Debug.Log("Engine exists. Please unload it first!");
            return;
        }

        // init engine
        mRtcEngine = IRtcEngine.GetEngine(appId);

        // enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }

    public void join(string channel)
    {
        Debug.Log("calling join (channel = " + channel + ")");

        if (mRtcEngine == null)
            return;

        // set callbacks (optional)
        mRtcEngine.OnJoinChannelSuccess = onJoinChannelSuccess;
        mRtcEngine.OnUserJoined = onUserJoined;
        mRtcEngine.OnUserOffline = onUserOffline;

        // enable video
        mRtcEngine.EnableVideo();

        // join channel
        mRtcEngine.JoinChannel(channel, null, currUserVideoInfo.uid);
    }

    public string getSdkVersion()
    {
        string ver = IRtcEngine.GetSdkVersion();
        return ver;
    }

    public void leave()
    {
        Debug.Log("calling leave");

        if (mRtcEngine == null)
            return;

        // leave channel
        mRtcEngine.LeaveChannel();
        // deregister video frame observers in native-c code
        mRtcEngine.DisableVideoObserver();
        // Clear user list of chat
        videoUserList.Clear();
        videoScreenList.Clear();
        videoUserList.Add(currUserVideoInfo);

    }

    // unload agora engine
    public void unloadEngine()
    {
        Debug.Log("calling unloadEngine");

        // delete
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();  // Place this call in ApplicationQuit
            mRtcEngine = null;
        }
    }

    public void EnableVideo(bool pauseVideo)
    {
        if (mRtcEngine != null)
        {
            if (!pauseVideo)
            {
                mRtcEngine.EnableVideo();
            }
            else
            {
                mRtcEngine.DisableVideo();
            }
        }
    }

    // implement engine callbacks
    private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);

        GameObject go = Instantiate(videoObject);
        go.name = currUserVideoInfo.uid.ToString();
        videoScreenList.Add(go);

        // allow camera output callback
        mRtcEngine.EnableVideoObserver();



        // GameObject textVersionGameObject = GameObject.Find("VersionText");
        // textVersionGameObject.GetComponent<Text>().text = "SDK Version : " + getSdkVersion();
    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    private void onUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);
        // this is called in main thread

        GameObject go = Instantiate(videoObject);
        go.name = uid.ToString();

        UserVideo uv = new UserVideo(uid, "Temp name", "Temp title");
        videoUserList.Add(uv);

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = go.GetComponent<VideoSurface>();
        if (!ReferenceEquals(videoSurface, null))
        {
            // configure videoSurface
            videoSurface.SetForUser(uid);
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            videoSurface.SetGameFps(30);
        }
        videoScreenList.Add(go);

        // 위치 이동
        for(int i = 0; i < videoUserList.Count ; i++)
        {
            videoScreenList[i].transform.position = videoPositionList[i];
        }
    }

    // When remote user is offline, this delegate will be called. Typically
    // delete the GameObject for this user
    private void onUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove video stream
        Debug.Log("onUserOffline: uid = " + uid + " reason = " + reason);
        // this is called in main thread
        GameObject go = GameObject.Find(uid.ToString());
        videoScreenList.Remove(go);
        // User List 삭제. UserVideo는 uid만 비교해서 동일한지 아닌지 판단함.
        videoUserList.Remove(new UserVideo(uid));
    }
}
