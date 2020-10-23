using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SummaryScroller : MonoBehaviour
{
    public TextMeshProUGUI textSummary;
    public float scrollSpeed = 50;
    public const float summaryColumnWidth = 380;
    public const float padding = 10;

    RectTransform textRectTransform;

    TextMeshProUGUI textSummaryClone;

    public bool hasTextChanged;

    void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
    }

    void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
    }

    void ON_TEXT_CHANGED(Object obj)
    {
        if (obj == textSummary.GetComponent<TMP_Text>())
        {
            hasTextChanged = true;
        }
    }

    public void SetTextChanged(bool isChanged)
    {
        hasTextChanged = isChanged;
    }

    void Start()
    {
        textRectTransform = textSummary.GetComponent<RectTransform>();
        textSummaryClone = Instantiate(textSummary);
        textSummaryClone.enabled = false;

        RectTransform cloneRectTransform = textSummaryClone.GetComponent<RectTransform>();
        cloneRectTransform.SetParent(textRectTransform);
        cloneRectTransform.anchorMin = new Vector2(1, 0.5f);
        cloneRectTransform.pivot = new Vector2(0, 0.5f);
        cloneRectTransform.anchoredPosition = new Vector3(padding, 0, 0);

        StartCoroutine(scrollText());
    }

    IEnumerator scrollText()
    {
        float width = textSummary.preferredWidth;
        Vector3 startPosition = textRectTransform.localPosition;
        float scrollPositionX = 0;

        while (true)
        {
            if (hasTextChanged)
            {
                width = textSummary.preferredWidth;
                if (width < summaryColumnWidth - (padding * 2))
                {
                    textRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    textRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    textRectTransform.pivot = new Vector2(0.5f, 0.5f);
                    textRectTransform.anchoredPosition = new Vector3(0, 0, 0);
                    textSummaryClone.enabled = false;
                }
                else
                {
                    textRectTransform.anchorMin = new Vector2(0, 0.5f);
                    textRectTransform.anchorMax = new Vector2(0, 0.5f);
                    textRectTransform.pivot = new Vector2(0, 0.5f);
                    textRectTransform.anchoredPosition = new Vector3(padding, 0, 0);
                    textSummaryClone.text = textSummary.text;
                    textSummaryClone.enabled = true;
                }

                startPosition = textRectTransform.localPosition;
                scrollPositionX = 0;

                hasTextChanged = false;
            }

            if (width >= summaryColumnWidth - (padding * 2))
            {
                textRectTransform.localPosition = new Vector3((-scrollPositionX % (width + padding)) + padding, startPosition.y, startPosition.z);
                scrollPositionX += scrollSpeed * Time.deltaTime;
            }

            yield return null;
        }
    }
}
