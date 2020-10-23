using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public InputField username;
    public InputField password;

    public void LoginScene()
    {
        StartCoroutine(LoginRequest());
    }
    IEnumerator LoginRequest()
    {
        UnityWebRequest loginPage = UnityWebRequest.Get("http://localhost:8000/api/token/");
        yield return loginPage.SendWebRequest();
        if (loginPage.isNetworkError || loginPage.isHttpError)
        {
            Debug.Log(loginPage.error);
            yield break;
        }

        // get the csrf cookie
        string SetCookie = loginPage.GetResponseHeader("set-cookie");
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

        if (doLogin.isNetworkError || doLogin.isHttpError)
        {
            Debug.Log(doLogin.error);
            yield break;
        }

        SceneManager.LoadScene("LoadingScene");
    }
}
