using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
public class GameManager : MonoBehaviour {

    [SerializeField] float InitialBallHeight;
    [SerializeField] Vector3 InitialSlime1Position;
    [SerializeField] Vector3 InitialSlime2Position;

    Transform ball;
    Transform slime1;
    Transform slime2;
    ScoreboardController scoreboard;
    Transform canvas;
    Text textPressSpace;
    Text textPlayerScored;
    Text textFinal;
    int[] scores = new int[2] { 0, 0 };
    private bool waitForSpace = false;

    void Awake() {
        ball = transform.Find("ball");
        slime1 = transform.Find("slime1");
        slime2 = transform.Find("slime2");
        scoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<ScoreboardController>();
        canvas = GameObject.FindGameObjectWithTag("Canvas").transform;
        textPressSpace = canvas.Find("PressSpaceText").GetComponent<Text>();
        textPlayerScored = canvas.Find("PlayerScoredText").GetComponent<Text>();
        textPlayerScored.gameObject.SetActive(false);
        textFinal = canvas.Find("FinalText").GetComponent<Text>();
        textFinal.gameObject.SetActive(false);
        Time.timeScale = 0f;
        
        // wait for space to start the game
    }

    void Update() {
        if (Input.GetButton("Jump")) {
            NewGame();
        }
    }

    void OnEnable() {
        Events.gInstance.gEvent += HandleEvent;
    }

    void OnDisable() {
        Events.gInstance.gEvent -= HandleEvent;
    }

    private void HandleEvent(string e, object[] p) {
        if (e == "round.end") {
            Time.timeScale = 0f;
            scores[(int)p[0]]++;
            scoreboard.SetScores(scores[0], scores[1]);
            if (scores[0] == scoreboard.MaxScore) {
                textFinal.text = "Player 1 wins!";
                textFinal.gameObject.SetActive(true);
                textPressSpace.gameObject.SetActive(true);
            } else if (scores[1] == scoreboard.MaxScore) {
                textFinal.text = "Player 2 wins!";
                textFinal.gameObject.SetActive(true);
                textPressSpace.gameObject.SetActive(true);
            } else {
                textPlayerScored.text = String.Format("Player {0} scored!", (int)p[0] + 1);
                textPlayerScored.gameObject.SetActive(true);
                RunDelayed(1f, () => {
                    textPlayerScored.gameObject.SetActive(false);
                    NewRound((RoundOutcome)p[0] == RoundOutcome.LeftScore);
                    Time.timeScale = 1f;
                });
            }
        }
    }

    private void NewGame() {
        scores[0] = scores[1] = 0;
        scoreboard.SetScores(scores[0], scores[1]);
        textPressSpace.gameObject.SetActive(false);
        textFinal.gameObject.SetActive(false);
        Time.timeScale = 1f;
        NewRound(true /* leftServes */);
    }

    private void NewRound(bool leftServes) {
        ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ball.position = ball.GetComponent<Rigidbody2D>().position = new Vector2((leftServes ? InitialSlime1Position : InitialSlime2Position).x, InitialBallHeight);
        slime1.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        slime1.position = slime1.GetComponent<Rigidbody2D>().position = InitialSlime1Position;
        slime2.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        slime2.position = slime2.GetComponent<Rigidbody2D>().position = InitialSlime2Position;
    }

    protected IEnumerator DelayedCoroutine(float delay, Action a) {
        yield return new WaitForSecondsRealtime(delay);
        a();
    }

    protected void RunDelayed(float delay, Action a) {
        StartCoroutine(DelayedCoroutine(delay, a));
    }
}