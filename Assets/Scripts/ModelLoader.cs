using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Siccity.GLTFUtility;


[Serializable]
public class Exhibit
{
    public int id;
    public string mesh;
    public string hash;

    public string name;
    public string summary;
    public string info;

    public string position_id;
    public float posx;
    public float posy;
    public float posz;
    public float roty;
}

[Serializable]
public class ExhibitList
{
    public Exhibit[] exhibits;
}


public class ModelLoader : MonoBehaviour
{
    private string SITE_ADDRESS = "http://untact-museum.herokuapp.com/";

    public GameObject firstPersonController;
    public ExhibitListController exhibitListController;
    public RectTransform progressBar;
    public Text log;

    public Button restartButton;
    public Button exitButton;

    private GameObject wrapper;
    private string filePath;

    private int requiredCount;
    private int loadedCount;
    private bool loadStatus;

    void Start()
    {
        requiredCount = -1;
        loadedCount = 0;
        log.text = "";
        filePath = $"{Application.persistentDataPath}/Assets/Models/";
        wrapper = new GameObject
        {
            name = "Exhibit"
        };
        SetLoadStatus(true);
        StartCoroutine(GetExhibitRequest());
    }

    void Update()
    {
        float percentage = requiredCount > 0 ? (float)loadedCount / requiredCount : 0f;
        progressBar.localScale = new Vector3(percentage, 1f, 1f);

        if (loadedCount == requiredCount)
        {
            exhibitListController.SetShowAll();
            exhibitListController.Show();
            firstPersonController.GetComponent<FirstPersonMovement>().enabled = true;
            firstPersonController.GetComponent<Jump>().enabled = true;
            firstPersonController.GetComponent<Crouch>().enabled = true;
            firstPersonController.GetComponent<MenuController>().enabled = true;
            firstPersonController.GetComponentInChildren<FirstPersonLook>().enabled = true;
            firstPersonController.GetComponentInChildren<Zoom>().enabled = true;
            firstPersonController.GetComponentInChildren<CollisionDetector>().enabled = true;
            gameObject.SetActive(false);
        }
    }

    void SetLoadStatus(bool state)
    {
        loadStatus = state;
        restartButton.gameObject.SetActive(!state);
        exitButton.gameObject.SetActive(!state);
    }

    public void RestartDownload()
    {
        log.text = "";
        SetLoadStatus(true);
    }

    public void ExitToLoginPage()
    {
        UnityWebRequest.ClearCookieCache();
        SceneManager.LoadScene("LoginScene");
    }

    string GetFilePath(string url)
    {
        string[] pieces = url.Split('/');
        string filename = pieces[pieces.Length - 1].Split('?')[0];

        return $"{filePath}{filename}";
    }

    IEnumerator GetModelRequest(Exhibit exhibit)
    {
        string url = exhibit.mesh;
        while (true)
        {
            UnityWebRequest req = UnityWebRequest.Get(url);
            req.downloadHandler = new DownloadHandlerFile(GetFilePath(url));
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                log.text = req.error;
                File.Delete(GetFilePath(url));
                SetLoadStatus(false);
                while (!loadStatus)
                {
                    yield return null;
                }
            }
            else
            {
                log.text = $"Downloaded file : {url.Split('?')[0]}";
                LoadModel(exhibit);
                break;
            }
        }

    }

    IEnumerator GetExhibitRequest()
    {
        while (true)
        {
            UnityWebRequest req = UnityWebRequest.Get(SITE_ADDRESS + "api/exhibit/");
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                log.text = req.error;
                SetLoadStatus(false);
                while (!loadStatus)
                {
                    yield return null;
                }
            }
            else
            {
                ExhibitList exhibitList = JsonUtility.FromJson<ExhibitList>("{\"exhibits\":" + req.downloadHandler.text + "}");
                requiredCount = exhibitList.exhibits.Length;
                foreach (Exhibit exhibit in exhibitList.exhibits)
                {
                    string path = GetFilePath(exhibit.mesh);
                    if (File.Exists(path))
                    {
                        MD5 md5 = MD5.Create();
                        StringBuilder hashBuilder = new StringBuilder();
                        String hash;
                        using (var file = File.OpenRead(path))
                        {
                            foreach (byte b in md5.ComputeHash(file))
                            {
                                hashBuilder.Append(b.ToString("x2"));
                            }
                            hash = hashBuilder.ToString();
                        }

                        if (exhibit.hash == hash)
                        {
                            log.text = $"Found file locally : {path}";
                            LoadModel(exhibit);
                            yield return null;
                        }
                        else
                        {
                            File.Delete(path);
                            yield return StartCoroutine(GetModelRequest(exhibit));
                        }
                    }
                    else
                    {
                        yield return StartCoroutine(GetModelRequest(exhibit));
                    }
                }
                break;
            }
        }

    }

    void LoadModel(Exhibit exhibit)
    {
        GameObject exhibitObject = Importer.LoadFromFile(GetFilePath(exhibit.mesh));
        ExhibitData data = exhibitObject.AddComponent<ExhibitData>();
        exhibitObject.AddComponent<MeshCollider>();

        exhibitObject.transform.SetParent(wrapper.transform);
        exhibitObject.transform.position = new Vector3(exhibit.posx, exhibit.posy, exhibit.posz);
        exhibitObject.transform.Rotate(0.0f, exhibit.roty, 0.0f);

        data.positionId = exhibit.position_id;
        data.name = exhibit.name;
        data.summary = exhibit.summary;
        data.info = exhibit.info;

        exhibitListController.GetExhibit(exhibitObject);
        loadedCount += 1;
    }
}