using System;
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
    public enum State { Login, Signup, SignupComplete };

    public InputField loginUsername;
    public InputField loginPassword;
    public Button loginButton;
    public Text loginNotice;

    public InputField signupUsername;
    public InputField signupPassword;
    public Button signupButton;
    public Text signupNotice;

    public GameObject loginPage;
    public GameObject signupPage;
    public GameObject signupCompletePage;

    private string csrfCookie;
    private State currentState;

    void Start()
    {
        ChangeState(State.Login);
    }

    public void Login()
    {
        StartCoroutine(LoginRequest());
    }

    public void Signup()
    {
        StartCoroutine(SignupRequest());
    }

    public void ChangeState(int state)
    {
        ChangeState((State)state);
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void ChangeState(State state)
    {
        currentState = state;
        loginPage.SetActive(false);
        signupPage.SetActive(false);
        signupCompletePage.SetActive(false);

        loginNotice.text = "";
        loginUsername.text = "";
        loginPassword.text = "";

        signupNotice.text = "";
        signupUsername.text = "";
        signupPassword.text = "";

        switch (state)
        {
            case State.Signup:
                signupPage.SetActive(true);
                break;
            case State.SignupComplete:
                signupCompletePage.SetActive(true);
                break;
            default:
                loginPage.SetActive(true);
                break;
        }
    }

    IEnumerator LoginRequest()
    {
        loginNotice.text = "";
        loginButton.enabled = false;
        string currentUsername = loginUsername.text;
        string currentPassword = loginPassword.text;

        if (String.IsNullOrEmpty(currentUsername) || String.IsNullOrEmpty(currentPassword))
        {
            loginButton.enabled = true;
            loginNotice.text = "Please fill out the form";
            yield break;
        }

        yield return StartCoroutine(TokenRequest());

        if (String.IsNullOrEmpty(csrfCookie))
        {
            loginButton.enabled = true;
            loginNotice.text = "Unable to connect to the server";
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("username", currentUsername);
        form.AddField("password", currentPassword);

        UnityWebRequest doLogin = UnityWebRequest.Post("http://localhost:8000/api/login/", form);

        doLogin.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        doLogin.SetRequestHeader("X-CSRFTOKEN", csrfCookie);

        yield return doLogin.SendWebRequest();

        loginButton.enabled = true;
        if (doLogin.isNetworkError) loginNotice.text = "Unable to connect to the server";
        else if (doLogin.isHttpError) loginNotice.text = "Username or password is incorrect";
        else SceneManager.LoadScene("MainScene");
    }

    IEnumerator SignupRequest()
    {
        signupNotice.text = "";
        signupButton.enabled = false;
        string currentUsername = signupUsername.text;
        string currentPassword = signupPassword.text;

        if (String.IsNullOrEmpty(currentUsername) || String.IsNullOrEmpty(currentPassword))
        {
            signupButton.enabled = true;
            signupNotice.text = "Please fill out the form";
            yield break;
        }

        if (currentUsername.Length < 4 || currentUsername.Length > 16)
        {
            signupButton.enabled = true;
            signupNotice.text = "Username must be 4-16 characters long.";
            yield break;
        }

        if (currentPassword.Length < 8 || currentPassword.Length > 128)
        {
            signupButton.enabled = true;
            signupNotice.text = "Password must be 8-128 characters long.";
            yield break;
        }

        yield return StartCoroutine(TokenRequest());

        if (csrfCookie == null)
        {
            signupButton.enabled = true;
            signupNotice.text = "Unable to connect to the server";
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("username", currentUsername);
        form.AddField("password", currentPassword);

        UnityWebRequest doSignup = UnityWebRequest.Post("http://localhost:8000/api/signup/", form);

        doSignup.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        doSignup.SetRequestHeader("X-CSRFTOKEN", csrfCookie);

        yield return doSignup.SendWebRequest();

        signupButton.enabled = true;
        if (doSignup.isNetworkError) signupNotice.text = "Unable to connect to the server";
        else if (doSignup.isHttpError) signupNotice.text = "This username is already taken";
        else
        {
            ChangeState(State.SignupComplete);
        }
    }

    IEnumerator TokenRequest()
    {
        UnityWebRequest getToken = UnityWebRequest.Get("http://localhost:8000/api/token/");
        yield return getToken.SendWebRequest();
        if (getToken.isNetworkError || getToken.isHttpError)
        {
            csrfCookie = null;
        }
        else
        {
            string SetCookie = getToken.GetResponseHeader("set-cookie");
            Regex rxCookie = new Regex("csrftoken=(?<csrf_token>.{64});");
            MatchCollection cookieMatches = rxCookie.Matches(SetCookie);
            csrfCookie = cookieMatches[0].Groups["csrf_token"].Value;
        }
    }
}
