using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject menu;
    public bool menuOn = false;

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
                menu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                gameObject.GetComponent<FirstPersonMovement>().enabled = true;
                gameObject.GetComponent<Jump>().enabled = true;
                gameObject.GetComponent<Crouch>().enabled = true;
                gameObject.GetComponentInChildren<FirstPersonLook>().enabled = true;
                gameObject.GetComponentInChildren<Zoom>().enabled = true;
                menu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
