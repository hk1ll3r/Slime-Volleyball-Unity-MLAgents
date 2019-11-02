using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScoreboardController : MonoBehaviour
{

    public int MaxScore; // 7
    Transform panelLeft;
    Transform panelRight;

    private void Awake()
    {
        panelLeft = transform.Find("ScorePanelLeft");
        panelRight = transform.Find("ScorePanelRight");
    }

    public void SetScores(int left, int right) {
        for (int i = 0; i < MaxScore; i++) {
            panelLeft.GetChild(i).GetComponent<ScoreRingController>().SetOn(i < left);
            panelRight.GetChild(MaxScore - i - 1).GetComponent<ScoreRingController>().SetOn(i < right);
        }
    }

}
