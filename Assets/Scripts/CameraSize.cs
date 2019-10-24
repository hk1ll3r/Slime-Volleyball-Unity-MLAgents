using UnityEngine;

public class CameraSize : MonoBehaviour {

    private int width, height;
    void Start() {
        width = Screen.width;
        height = Screen.height;
        RefreshCameraSize();
    }

    void Update() {
        if (Screen.width != width || Screen.height != height) {
            RefreshCameraSize();
            width = Screen.width;
            height = Screen.height;
        }
    }

    public void RefreshCameraSize() {
        Camera cam = Camera.main;
        float aspect = (float)Screen.width / (float)Screen.height;
        if (aspect > 2f) {
            cam.orthographicSize = 3.75f;
        } else {
            cam.orthographicSize = 7.5f / aspect;
        }
        //Debug.Log("orth size: " + cam.orthographicSize);
    }
}