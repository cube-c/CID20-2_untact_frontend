using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
#if(UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif
using System.Collections;

/// <summary>
///    TestHome serves a game controller object for this application.
/// </summary>
public class VideoChatManager : MonoBehaviour
{

    public string userID;

    // Use this for initialization
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif
    static UntactVideoChat app = null;

    private string LoginSceneName = "LoginScene";

    private string MainSceneName = "MainScene";

    // PLEASE KEEP THIS App ID IN SAFE PLACE
    // Get your own App ID at https://dashboard.agora.io/
    [SerializeField]
    private string AppID = "f4cca77d4c724833bbccbfef51f48290";

    void Awake()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
		permissionList.Add(Permission.Microphone);         
		permissionList.Add(Permission.Camera);               
#endif

        // keep this alive across scenes
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        //CheckAppId();
    }

    void Update()
    {
        CheckPermissions();
    }

    // AppID 가 제대로 되었는지 확인하는 부분. 필요할 수 있으므로 주석처리 해 둠.
    //private void CheckAppId()
    //{
    //    Debug.Assert(AppID.Length > 10, "Please fill in your AppId first on Game Controller object.");
    //    GameObject go = GameObject.Find("AppIDText");
    //    if (go != null)
    //    {
    //        Text appIDText = go.GetComponent<Text>();
    //        if (appIDText != null)
    //        {
    //            if (string.IsNullOrEmpty(AppID))
    //            {
    //                appIDText.text = "AppID: " + "UNDEFINED!";
    //            }
    //            else
    //            {
    //                appIDText.text = "AppID: " + AppID.Substring(0, 4) + "********" + AppID.Substring(AppID.Length - 4, 4);
    //            }
    //        }
    //    }
    //}

    /// <summary>
    ///   Checks for platform dependent permissions.
    /// </summary>
    private void CheckPermissions()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        foreach(string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {                 
				Permission.RequestUserPermission(permission);
			}
        }
#endif
    }

    public void LoadMainsceneAfterLogin(string userid)
    {
        userID = userid;

        // create app if nonexistent
        if (ReferenceEquals(app, null))
        {
            app = new UntactVideoChat(); // create app
            app.loadEngine(AppID); // load engine
        }

        // join channel and jump to next scene
        app.join(userID);
        SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        SceneManager.LoadScene(MainSceneName, LoadSceneMode.Single);
    }

    public void onLeaveButtonClicked()
    {
        if (!ReferenceEquals(app, null))
        {
            app.leave(); // leave channel
            app.unloadEngine(); // delete engine
            app = null; // delete app
            SceneManager.LoadScene(LoginSceneName, LoadSceneMode.Single);
        }
        Destroy(gameObject);
    }

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == MainSceneName)
        {
            if (!ReferenceEquals(app, null))
            {
                app.onSceneHelloVideoLoaded(); // call this after scene is loaded
            }
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }
    }

    void OnApplicationPause(bool paused)
    {
        if (!ReferenceEquals(app, null))
        {
            app.EnableVideo(paused);
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
