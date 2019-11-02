using UnityEngine;
using UnityEngine.Events;

public class ScoreRingController : MonoBehaviour
{
    Transform inner;

    private void Awake()
    {
        inner = transform.Find("ScoreInner");
        SetOn(false);
    }

    public void SetOn(bool on) {
        inner.gameObject.SetActive(on);
    }

}
