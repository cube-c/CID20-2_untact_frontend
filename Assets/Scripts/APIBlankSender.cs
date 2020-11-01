using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class APIBlankSender : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(BlankRequest());
    }

    IEnumerator BlankRequest()
    {
        while (true)
        {
            using (UnityWebRequest req = UnityWebRequest.Get("http://localhost:8000/api/blank/"))
            {
                yield return req.SendWebRequest();

                if (req.isNetworkError || req.isHttpError)
                {
                    Debug.Log(req.error);
                }
                else
                {
                    //Debug.Log("ping");
                }
            }

            yield return new WaitForSeconds(15);
        }
    }
}
