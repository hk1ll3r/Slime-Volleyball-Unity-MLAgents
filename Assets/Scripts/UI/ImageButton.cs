using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ImageButton : MonoBehaviour
{
    [SerializeField] Sprite spriteOn;
    [SerializeField] Sprite spriteOff;
    [SerializeField] string eventName;

    Image image;

    private bool on = false;

    // Start is called before the first frame update
    void Start()
    {
        image = transform.Find("Image").GetComponent<Image>();
        UpdateSprite();
    }

    void OnEnable()
    {
        Events.gInstance.gEvent += HandleEvent;
    }

    void OnDisable()
    {
        Events.gInstance.gEvent -= HandleEvent;
    }

    void HandleEvent(string e, params object[] p)
    {
        if (eventName == e)
        {
            on = (bool)p[0];
            UpdateSprite();
        }
    }

    // Update is called once per frame
    void UpdateSprite()
    {
        image.sprite = on ? spriteOn : spriteOff;
    }
}
