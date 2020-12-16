using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using agora_gaming_rtc;
using agora_utilities;
public class UserVideo : MonoBehaviour
{
    public string name;
    public string title;
    public uint uid;

    public UserVideo(uint uid, string name, string title )
    {
        this.name = name;
        this.uid = uid;
        this.title = title;
    }

    public UserVideo(uint uid)
    {
        this.name = "temp";
        this.uid = uid;
        this.title = "temp";
    }

    public bool Equals(UserVideo uv)
    {
        if (this.uid == uv.uid) return true;
        else return false;
    }
}
