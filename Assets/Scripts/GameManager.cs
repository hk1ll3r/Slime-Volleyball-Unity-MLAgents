using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using MLAgents;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float roundTimeLimit; // round ends with no rewards if this time limit reaches 

    [SerializeField] float InitialBallHeight;
    [SerializeField] float InitialSlimeX;

    Rigidbody2D ball;
    Rigidbody2D slime1;
    Rigidbody2D slime2;
    Rigidbody2D ground;

    [SerializeField] Brain brainAI;
    [SerializeField] Brain brainP1;
    [SerializeField] Brain brainP2;

    private float roundStartTime;

    ScoreboardController scoreboard;
    Transform canvas;
    Text textPlayerScored;
    Text textPaused;
    Text textFinal;
    Text textRoundTimer;
    int[] scores = new int[2] { 0, 0 };

    private bool mPhysicsPaused;
    public bool IsPhysicsPaused
    {
        get { return mPhysicsPaused; }
    }

    void Awake()
    {
        ground = transform.Find("ground").GetComponent<Rigidbody2D>();
        ball = transform.Find("ball").GetComponent<Rigidbody2D>();
        slime1 = transform.Find("slime1").GetComponent<Rigidbody2D>();
        slime2 = transform.Find("slime2").GetComponent<Rigidbody2D>();

        scoreboard = GameObject.FindGameObjectWithTag("Scoreboard").GetComponent<ScoreboardController>();
        canvas = GameObject.FindGameObjectWithTag("Canvas").transform;
        textPaused = canvas.Find("PausedText").GetComponent<Text>();
        textPlayerScored = canvas.Find("PlayerScoredText").GetComponent<Text>();
        textPlayerScored.gameObject.SetActive(false);
        textFinal = canvas.Find("FinalText").GetComponent<Text>();
        textFinal.gameObject.SetActive(false);
        textRoundTimer = canvas.Find("RoundTimerText").GetComponent<Text>();
        textRoundTimer.gameObject.SetActive(false);

        // wait for space to start the game
        Time.timeScale = 0f;
        Menu();
    }

    private void FixedUpdate()
    {
        if (IsPhysicsPaused) return;

        float remainingTime = roundStartTime + roundTimeLimit - Time.fixedTime;
        if (remainingTime <= 0f)
        {
            textRoundTimer.text = "Round Time 0:00";
            OnRoundEnd(RoundOutcome.None);
        } else
        {
            textRoundTimer.text = string.Format("Round Time {0:D}:{1:D2}", Mathf.CeilToInt(remainingTime) / 60, Mathf.CeilToInt(remainingTime) % 60);
        }
    }

    /*public Vector2 groundPosition
    {
        get { return meSlime.position - ground.position; }
        set { meSlime.position = value + ground.position; }
    }*/

    // TODO implement save / load?
    /*private GameState lastGameState;
    public GameState CurGameState
    {
        get
        {
            return null;
        }
        set
        {

        }
    }*/

    private void PausePhysics()
    {
        mPhysicsPaused = true;
        ball.simulated = false;
        ball.velocity = Vector2.zero;
        slime1.velocity = Vector2.zero;
        slime2.velocity = Vector2.zero;
    }

    private void ResumePhysics()
    {
        mPhysicsPaused = false;
        ball.simulated = true;
    }

    void OnEnable()
    {
        Events.gInstance.gEvent += HandleEvent;
    }

    void OnDisable()
    {
        Events.gInstance.gEvent -= HandleEvent;
    }

    public void OnRoundEnd(RoundOutcome outcome)
    {
        int scoredPlayer;
        switch (outcome)
        {
            case RoundOutcome.LeftScore:
                scoredPlayer = 0;
                scores[scoredPlayer]++;
                break;
            case RoundOutcome.RightScore:
                scoredPlayer = 1;
                scores[scoredPlayer]++;
                break;
            default:
                scoredPlayer = -1;
                break;
        }
        scoreboard.SetScores(scores[0], scores[1]);
        if (scores[0] == scoreboard.MaxScore)
        {
            End("Player 1 wins!");
        }
        else if (scores[1] == scoreboard.MaxScore)
        {
            End("Player 2 wins!");
        }
        else
        {
            if (scoredPlayer == -1) textPlayerScored.text = "No Player scored!";
            else textPlayerScored.text = String.Format("Player {0} scored!", scoredPlayer + 1);
            textPlayerScored.gameObject.SetActive(true);
            PausePhysics();
            RunDelayed(1f, () =>
            {
                ResumePhysics();
                textPlayerScored.gameObject.SetActive(false);
                bool leftServes;
                if (outcome == RoundOutcome.None)
                {
                    leftServes = (UnityEngine.Random.Range(0, 2) == 0);
                } else leftServes = (outcome == RoundOutcome.LeftScore);
                NewRound(leftServes);
            });
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        UIAction.SetEnabledActions(new string[] { "resume", "end" });
        textPaused.gameObject.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        UIAction.SetEnabledActions(new string[] { "pause" });
        textPaused.gameObject.SetActive(false);
    }

    public void End(string result)
    {
        ResumePhysics();
        StopAllCoroutines();
        Time.timeScale = 0f;
        UIAction.SetEnabledActions(new string[] { "start", "menu" });
        textFinal.text = result;
        textFinal.gameObject.SetActive(true);
        textPaused.gameObject.SetActive(false);
        textPlayerScored.gameObject.SetActive(false);
        textRoundTimer.gameObject.SetActive(false);
    }

    public void Menu()
    {
        ResetState(true);
        UIAction.SetEnabledActions(new string[] { "start", "control1", "control2" });
        scores[0] = scores[1] = 0;
        scoreboard.SetScores(scores[0], scores[1]);
        textFinal.gameObject.SetActive(false);
        textPaused.gameObject.SetActive(false);
    }

    private void Control(Player player)
    {
        SlimeAgent agent = (player == Player.One ? slime1 : slime2).GetComponent<SlimeAgent>();
        string playerStr = player == Player.One ? "1" : "2";
        if (agent.brain == brainAI)
        {
            agent.brain = player == Player.One ? brainP1 : brainP2;
            agent.agentParameters.numberOfActionsBetweenDecisions = 1;
            Events.gInstance.RaiseEventImmediate(string.Format("ui.control{0}.ai", playerStr), false);
        } else {
            agent.brain = brainAI;
            agent.agentParameters.numberOfActionsBetweenDecisions = 2;
            Events.gInstance.RaiseEventImmediate(string.Format("ui.control{0}.ai", playerStr), true);
        }
    }

    private void HandleEvent(string e, object[] p)
    {
        Debug.LogFormat("game manager event: {0}", e);
        if (e.StartsWith("ui."))
        {
            string uiAction = e.Substring(3);
            Debug.LogFormat("uiaction: {0}", uiAction);

            if (uiAction == "start")
            {
                NewGame();
            }
            else if (uiAction == "end")
            {
                End("No winner :|");
            }
            else if (uiAction == "pause")
            {
                Pause();
            }
            else if (uiAction == "resume")
            {
                Resume();
            }
            else if (uiAction == "menu")
            {
                Menu();
            } else if (uiAction == "control1")
            {
                Control(Player.One);
            }
            else if (uiAction == "control2")
            {
                Control(Player.Two);
            }
        }
    }

    private void NewGame()
    {
        UIAction.SetEnabledActions(new string[] { "pause" });
        scores[0] = scores[1] = 0;
        scoreboard.SetScores(scores[0], scores[1]);
        textFinal.gameObject.SetActive(false);
        textRoundTimer.gameObject.SetActive(true);
        Time.timeScale = 1f;
        NewRound(true /* leftServes */);
    }

    private void NewRound(bool leftServes)
    {
        roundStartTime = Time.fixedTime;
        ResetState(leftServes);
    }

    private void ResetState(bool leftServes)
    {
        ball.velocity = Vector2.zero;
        ball.transform.localPosition = new Vector2((leftServes ? -1 : 1) * InitialSlimeX, ground.transform.localPosition.y + InitialBallHeight);
        ball.position = ball.transform.position;
        slime1.velocity = Vector2.zero;
        slime1.transform.localPosition = new Vector2(-InitialSlimeX, ground.transform.localPosition.y);
        slime1.position = slime1.transform.position;
        slime2.velocity = Vector2.zero;
        slime2.transform.localPosition = new Vector2(InitialSlimeX, ground.transform.localPosition.y);
        slime2.position = slime2.transform.position;
    }

    protected IEnumerator DelayedCoroutine(float delay, Action a)
    {
        yield return new WaitForSeconds(delay);
        a();
    }

    protected void RunDelayed(float delay, Action a)
    {
        StartCoroutine(DelayedCoroutine(delay, a));
    }
}