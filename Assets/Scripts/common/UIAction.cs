using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIAction : MonoBehaviour
{
    [SerializeField] private string actionName;
    [SerializeField] private KeyCode[] keycodes;
    [SerializeField] private string eventName;
    [SerializeField] private string description;

    public readonly static Dictionary<string, UIAction> allActions = new Dictionary<string, UIAction>();
    public readonly static Dictionary<string, UIAction> enabledActions = new Dictionary<string, UIAction>();
    public static void SetActionEnabled(string actionName, bool enabled) {
        UIAction mThis = allActions[actionName];
        mThis.gameObject.SetActive(enabled);
    }

    public static void SetEnabledActions(string[] actionNames)
    {
        foreach (string k in enabledActions.Keys.ToArray()) {
            SetActionEnabled(k, false);
        }

        foreach (string k in actionNames)
        {
            SetActionEnabled(k, true);
        }

        GameObject ad = GameObject.FindGameObjectWithTag("ActionDescription");
        if (ad != null)
        {
            string d = "";
            foreach (UIAction a in enabledActions.Values)
            {
                d += "\n";
                d += a.description;
            }
            ad.GetComponent<Text>().text = d;
        }
    }

    private Button button;

    private void Awake()
    {
        allActions[actionName] = this;
        button = GetComponent<Button>();
    }

    void OnEnable()
    {
        enabledActions[actionName] = (this);
        if (button != null)
        {
            button.onClick.AddListener(() => { Fire(); });
        }
    }

    private void Update()
    {
        if (keycodes.Length == 0) return;

        foreach (KeyCode kc in keycodes)
        {
            if (Input.GetKeyDown(kc))
            {
                Fire();
                break;
            }
        }
    }

    public void Fire()
    {
        Events.gInstance.RaiseEventImmediate(eventName);
    }

    // Update is called once per frame
    void OnDisable()
    {
        if (button != null) { 
            button.onClick.RemoveAllListeners();
        }
        enabledActions.Remove(actionName);
    }
}
