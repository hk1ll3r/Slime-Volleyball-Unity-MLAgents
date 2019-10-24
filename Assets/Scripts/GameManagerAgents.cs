using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MLAgents;
public class GameManagerAgents : MonoBehaviour {

    [SerializeField] private int gameID; // to track multiple games in the same environment
    public int GameID {
        get { return gameID; }
    }
    [SerializeField] float InitialBallHeight;
    [SerializeField] float InitialSlimeX;

    Transform ball;
    Transform slime1;
    Transform slime2;
    Transform ground;

    void Awake() {
        ground = transform.Find("ground");
        ball = transform.Find("ball");
        slime1 = transform.Find("slimeAI1");
        slime2 = transform.Find("slimeAI2");
    }

    public void OnRoundEnd(RoundOutcome outcome) {
        Agent agent1 = slime1.GetComponent<SlimeAgent>();
        Agent agent2 = slime2.GetComponent<SlimeAgent>();
        float leftReward = (outcome == RoundOutcome.LeftScore) ? 5 : -5;
        agent1.SetReward(leftReward);
        agent2.SetReward(-leftReward);
        agent1.Done();
        agent2.Done();
        NewGame();
    }

    private void NewGame() {
        NewRound(Random.Range(0, 2) == 0);
    }

    private void NewRound(bool leftServes) {
        ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ball.localPosition = new Vector2((leftServes ? -1 : 1) * InitialSlimeX, ground.localPosition.y + InitialBallHeight);
        ball.GetComponent<Rigidbody2D>().position = ball.position;
        slime1.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        slime1.localPosition = new Vector2(-InitialSlimeX, ground.localPosition.y);
        slime1.GetComponent<Rigidbody2D>().position = slime1.position;
        slime2.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        slime2.localPosition = new Vector2(InitialSlimeX, ground.localPosition.y);
        slime2.GetComponent<Rigidbody2D>().position = slime2.position;
    }

}