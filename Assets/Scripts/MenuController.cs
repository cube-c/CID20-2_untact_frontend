using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.EventSystems;


[Serializable]
public class MyInfo
{
    public string user_name;
    public string user_title;
}


public class MenuController : MonoBehaviour
{
    public GameObject menu;
    public Button buttonUser;
    public Button buttonExhibit;
    public Toggle toggleDND;
    public GameObject userWindow;
    public Image userWindowBar;
    public GameObject exhibitWindow;
    public Image exhibitWindowBar;
    public bool menuOn = false;
    public bool userListOn = false;
    public bool exhibitListOn = false;

    public MyInfo myInfo;
    public Text textMyID;
    public Text textMyTitle;

    public GameObject userListController;


    void Start()
    {
        SwitchDND();
        StartCoroutine(GetInfoRequest());
    }

    IEnumerator GetInfoRequest()
    {
        using (UnityWebRequest getInfo = UnityWebRequest.Get("http://localhost:8000/api/myInfo/"))
        {
            yield return getInfo.SendWebRequest();

            if (getInfo.isNetworkError || getInfo.isHttpError)
            {
                Debug.Log(getInfo.error);
                yield break;
            }
            else
            {
                myInfo = JsonUtility.FromJson<MyInfo>(getInfo.downloadHandler.text);
                textMyID.text = myInfo.user_name;
                textMyTitle.text = myInfo.user_title;
            }
        }
    }

    public void Logout()
    {
        menu.SetActive(false);
        StartCoroutine(LogoutRequest());
    }

    IEnumerator LogoutRequest()
    {
        UnityWebRequest getToken = UnityWebRequest.Get("http://localhost:8000/api/token/");
        yield return getToken.SendWebRequest();
        if (getToken.isNetworkError || getToken.isHttpError)
        {
            Debug.Log(getToken.error);
            yield break;
        }

        // get the csrf cookie
        string SetCookie = getToken.GetResponseHeader("set-cookie");
        Regex rxCookie = new Regex("csrftoken=(?<csrf_token>.{64});");
        MatchCollection cookieMatches = rxCookie.Matches(SetCookie);
        string csrfCookie = cookieMatches[0].Groups["csrf_token"].Value;

        UnityWebRequest doLogout = UnityWebRequest.Post("http://localhost:8000/api/logout/", "");

        doLogout.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        doLogout.SetRequestHeader("X-CSRFTOKEN", csrfCookie);

        yield return doLogout.SendWebRequest();

        if (doLogout.isNetworkError || doLogout.isHttpError)
        {
            Debug.Log(doLogout.error);
            yield break;
        }

        SceneManager.LoadScene("LoginScene");
    }

    public void SwitchDND()
    {
        toggleDND.enabled = false;
        StartCoroutine(dndSwitchRequest(toggleDND.isOn));
    }

    IEnumerator dndSwitchRequest(bool dndIsOn)
    {
        UnityWebRequest getToken = UnityWebRequest.Get("http://localhost:8000/api/token/");
        yield return getToken.SendWebRequest();
        if (getToken.isNetworkError || getToken.isHttpError)
        {
            Debug.Log(getToken.error);
            yield break;
        }

        // get the csrf cookie
        string SetCookie = getToken.GetResponseHeader("set-cookie");
        Regex rxCookie = new Regex("csrftoken=(?<csrf_token>.{64});");
        MatchCollection cookieMatches = rxCookie.Matches(SetCookie);
        string csrfCookie = cookieMatches[0].Groups["csrf_token"].Value;

        WWWForm form = new WWWForm();
        form.AddField("dndswitch", dndIsOn.ToString());

        UnityWebRequest dndSwitch = UnityWebRequest.Post("http://localhost:8000/api/dndSwitch/", form);

        dndSwitch.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        dndSwitch.SetRequestHeader("X-CSRFTOKEN", csrfCookie);

        yield return dndSwitch.SendWebRequest();

        if (dndSwitch.isNetworkError || dndSwitch.isHttpError)
        {
            Debug.Log(dndSwitch.error);
            yield break;
        }

        toggleDND.enabled = true;
    }

    public void UserList()
    {
        userListOn = !userListOn;
        userWindow.SetActive(userListOn);

        ColorBlock cb = buttonUser.colors;

        if (userListOn)
        {
            userWindowBar.color = new Color(150f / 255, 150f / 255, 150f / 255, 100f / 255);
            userWindow.transform.SetAsLastSibling();
            exhibitWindowBar.color = new Color(200f / 255, 200f / 255, 200f / 255, 100f / 255);
            cb.normalColor = new Color(150f / 255, 150f / 255, 150f / 255, 1);
            cb.selectedColor = new Color(150f / 255, 150f / 255, 150f / 255, 1);
            cb.highlightedColor = new Color(180f / 255, 180f / 255, 180f / 255, 1);
            buttonUser.colors = cb;
            userListController.GetComponent<UserListController>().Refresh();
        }
        else
        {
            cb.normalColor = Color.white;
            cb.selectedColor = Color.white;
            cb.highlightedColor = new Color(220f / 255, 220f / 255, 220f / 255, 1);
            buttonUser.colors = cb;
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ExhibitList()
    {
        exhibitListOn = !exhibitListOn;
        exhibitWindow.SetActive(exhibitListOn);

        ColorBlock cb = buttonExhibit.colors;

        if (exhibitListOn)
        {
            exhibitWindowBar.color = new Color(150f / 255, 150f / 255, 150f / 255, 100f / 255);
            exhibitWindow.transform.SetAsLastSibling();
            userWindowBar.color = new Color(200f / 255, 200f / 255, 200f / 255, 100f / 255);
            cb.normalColor = new Color(150f / 255, 150f / 255, 150f / 255, 1);
            cb.selectedColor = new Color(150f / 255, 150f / 255, 150f / 255, 1);
            cb.highlightedColor = new Color(180f / 255, 180f / 255, 180f / 255, 1);
            buttonExhibit.colors = cb;
        }
        else
        {
            cb.normalColor = Color.white;
            cb.selectedColor = Color.white;
            cb.highlightedColor = new Color(220f / 255, 220f / 255, 220f / 255, 1);
            buttonExhibit.colors = cb;
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuOn = !menuOn;
            if (menuOn)
            {
                gameObject.GetComponent<FirstPersonMovement>().enabled = false;
                gameObject.GetComponent<Jump>().enabled = false;
                gameObject.GetComponent<Crouch>().enabled = false;
                gameObject.GetComponentInChildren<FirstPersonLook>().enabled = false;
                gameObject.GetComponentInChildren<Zoom>().enabled = false;
                gameObject.GetComponentInChildren<CollisionDetector>().enabled = false;
                menu.SetActive(true);
                userWindow.SetActive(userListOn);
                exhibitWindow.SetActive(exhibitListOn);
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                gameObject.GetComponent<FirstPersonMovement>().enabled = true;
                gameObject.GetComponent<Jump>().enabled = true;
                gameObject.GetComponent<Crouch>().enabled = true;
                gameObject.GetComponentInChildren<FirstPersonLook>().enabled = true;
                gameObject.GetComponentInChildren<Zoom>().enabled = true;
                gameObject.GetComponentInChildren<CollisionDetector>().enabled = true;
                menu.SetActive(false);
                userWindow.SetActive(false);
                exhibitWindow.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
