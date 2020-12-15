using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using agora_gaming_rtc;
using agora_utilities;
public class UserVideo : MonoBehaviour
{
    public string name;
    public string title;
    public int uid;

    public UserVideo(int uid, string name, string title )
    {
        this.name = name;
        this.uid = uid;
        this.title = title;
    }
}
