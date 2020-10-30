using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public InputField username;
    public InputField password;
    public Button loginButton;
    public Text loginNotice;

    public void Login()
    {
        StartCoroutine(LoginRequest());
    }

    IEnumerator LoginRequest()
    {
        loginButton.enabled = false;
        UnityWebRequest getToken = UnityWebRequest.Get("http://localhost:8000/api/token/");
        yield return getToken.SendWebRequest();
        if (getToken.isNetworkError || getToken.isHttpError)
        {
            loginButton.enabled = true;
            loginNotice.text = "Unable to connect to the server";
            yield break;
        }

        // get the csrf cookie
        string SetCookie = getToken.GetResponseHeader("set-cookie");
        Regex rxCookie = new Regex("csrftoken=(?<csrf_token>.{64});");
        MatchCollection cookieMatches = rxCookie.Matches(SetCookie);
        string csrfCookie = cookieMatches[0].Groups["csrf_token"].Value;

        WWWForm form = new WWWForm();
        form.AddField("username", username.text);
        form.AddField("password", password.text);

        UnityWebRequest doLogin = UnityWebRequest.Post("http://localhost:8000/api/login/", form);

        doLogin.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        doLogin.SetRequestHeader("X-CSRFTOKEN", csrfCookie);

        yield return doLogin.SendWebRequest();

        if (doLogin.isNetworkError)
        {
            loginButton.enabled = true;
            loginNotice.text = "Unable to connect to the server";
            yield break;
        }
        else if (doLogin.isHttpError)
        {
            loginButton.enabled = true;
            loginNotice.text = "Username or password is incorrect";
            yield break;
        }

        SceneManager.LoadScene("MainScene");
    }
}
