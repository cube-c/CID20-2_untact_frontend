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
    public string name;
    public string mesh;
    public string info;
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
    GameObject wrapper;
    string filePath;

    private void Start()
    {
        filePath = $"{Application.persistentDataPath}/Assets/Models/";
        wrapper = new GameObject
        {
            name = "Model"
        };
        StartCoroutine(GetRequest("http://localhost:8000/api/exhibit/"));
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

    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error);
            }
            else
            {
                ExhibitList list = JsonUtility.FromJson<ExhibitList>("{\"exhibits\":" + req.downloadHandler.text + "}");
                foreach(Exhibit exhibit in list.exhibits)
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
        GameObject model = Importer.LoadFromFile(GetFilePath(exhibit.mesh));
        model.transform.SetParent(wrapper.transform);
        model.transform.position = new Vector3(exhibit.posx, exhibit.posy, exhibit.posz);
        model.transform.Rotate(0.0f, exhibit.roty, 0.0f);
        ModelInfo info = model.AddComponent<ModelInfo>();
        MeshCollider collider = model.AddComponent<MeshCollider>();
        info.modelName = exhibit.name;
        info.modelInfo = exhibit.info;
    }
}