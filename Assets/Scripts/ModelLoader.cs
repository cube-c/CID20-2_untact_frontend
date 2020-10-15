using System;
using System.IO;
using System.Collections;
using UnityEngine;
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
}

[Serializable]
public class ExhibitList
{
    public Exhibit[] exhibits;
}

[Serializable]
public class Position
{
    public float posx;
    public float posy;
    public float posz;
    public float roty;
}


public class ModelLoader : MonoBehaviour
{
    GameObject wrapper;
    string filePath;

    private void Start()
    {
        filePath = $"{Application.persistentDataPath}/Assets/Models/";
        wrapper = new GameObject
        {
            name = "Exhibit"
        };
        StartCoroutine(GetExhibitRequest());
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
                Debug.Log(req.error);
                // Delete wrongly cached file
                File.Delete(GetFilePath(url));
            }
            else
            {
                Debug.Log($"Downloaded file : {url}");
                // Save the model
                LoadModel(exhibit);
            }
        }
    }

    IEnumerator GetPositionRequest(GameObject exhibitObject, string position_id)
    {
        using (UnityWebRequest req = UnityWebRequest.Get("http://localhost:8000/api/position/" + position_id))
        {
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error);
            }
            else
            {
                string text = req.downloadHandler.text;
                ExhibitData data = exhibitObject.GetComponent<ExhibitData>();
                data.position = JsonUtility.FromJson<Position>(text.Substring(1, text.Length - 2));

                exhibitObject.transform.SetParent(wrapper.transform);
                exhibitObject.transform.position = new Vector3(data.position.posx, data.position.posy, data.position.posz);
                exhibitObject.transform.Rotate(0.0f, data.position.roty, 0.0f);
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
                Debug.Log(req.error);
            }
            else
            {
                ExhibitList exhibitList = JsonUtility.FromJson<ExhibitList>("{\"exhibits\":" + req.downloadHandler.text + "}");
                foreach (Exhibit exhibit in exhibitList.exhibits)
                {
                    string path = GetFilePath(exhibit.mesh);
                    if (File.Exists(path))
                    {
                        Debug.Log($"Found file locally : {path}");
                        LoadModel(exhibit);
                    }
                    else
                    {
                        StartCoroutine(GetModelRequest(exhibit));
                    }
                }
            }
        }
    }

    void LoadModel(Exhibit exhibit)
    {
        GameObject exhibitObject = Importer.LoadFromFile(GetFilePath(exhibit.mesh));
        ExhibitData data = exhibitObject.AddComponent<ExhibitData>();
        StartCoroutine(GetPositionRequest(exhibitObject, exhibit.position_id));
        MeshCollider collider = exhibitObject.AddComponent<MeshCollider>();
        data.position_id = exhibit.position_id;
        data.name = exhibit.name;
        data.summary = exhibit.summary;
        data.info = exhibit.info;
    }
}