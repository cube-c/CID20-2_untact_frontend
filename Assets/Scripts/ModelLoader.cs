using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Siccity.GLTFUtility;


[Serializable]
public class Exhibit
{
    public int id;
    public string mesh;

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
    public ExhibitListController exhibitListController;
    public RectTransform progressBar;
    public Text log;

    private GameObject wrapper;
    private string filePath;

    private int requiredCount;
    private int loadedCount;

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
            gameObject.SetActive(false);
        }
    }

    string GetFilePath(string url)
    {
        string[] pieces = url.Split('/');
        string filename = pieces[pieces.Length - 1];

        return $"{filePath}{filename}";
    }

    IEnumerator GetModelRequest(Exhibit exhibit)
    {
        string url = "http://localhost:8000/media/" + exhibit.mesh;
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.downloadHandler = new DownloadHandlerFile(GetFilePath(url));
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                log.text = req.error;
                // Delete wrongly cached file
                File.Delete(GetFilePath(url));
            }
            else
            {
                log.text = $"Downloaded file : {url}";
                LoadModel(exhibit);
            }
        }
    }

    IEnumerator GetExhibitRequest()
    {
        using (UnityWebRequest req = UnityWebRequest.Get("http://localhost:8000/api/exhibit/"))
        {
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                log.text = req.error;
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
                        log.text = $"Found file locally : {path}";
                        LoadModel(exhibit);
                        yield return null;
                    }
                    else
                    {
                        yield return StartCoroutine(GetModelRequest(exhibit));
                    }
                }
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