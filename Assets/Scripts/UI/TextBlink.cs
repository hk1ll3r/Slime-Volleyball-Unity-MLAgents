using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextBlink : MonoBehaviour
{
    [SerializeField] float interval;

    Text text;
    bool curBlink;
    private float curTime;

    void Start()
    {
        text = GetComponent<Text>();
        SetBlink(true);
        curTime = 0f;
    }

    private void SetBlink(bool b)
    {
        curBlink = b;
        text.enabled = b;
    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.unscaledDeltaTime;
        if (curTime >= interval)
        {
            curTime = 0f;
            SetBlink(!curBlink);
        }
    }
}
