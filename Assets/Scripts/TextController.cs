using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextController : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public Scrollbar textScrollbar;
    public MenuController menuController;

    public void print(string text)
    {
        List<string> messageTextList = messageText.text.Split('\n').ToList<string>();
        messageTextList.Add(text);
        if (messageTextList.Count > 50)
        {
            messageTextList.RemoveAt(0);
        }
        messageText.text = string.Join("\n", messageTextList.ToArray());
        menuController.RefreshTextScrollBar();
        textScrollbar.value = 0;
    }
}
