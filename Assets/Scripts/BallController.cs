using UnityEngine;
using UnityEngine.Events;

public class BallController : MonoBehaviour
{
    const float gMaxVX = 11.25f; // org 15
    const float gMaxVY = 8.25f; // org 11
    
    public static Vector2 MaxV
    {
        get { return new Vector2(gMaxVX, gMaxVY); }
    }

    private Rigidbody2D mRigidbody2D;
    private Transform ground;
    private Transform slime1;
    private Transform slime2;
    private GameManagerAgents gameManager;

    private void Awake()
    {
        mRigidbody2D = GetComponent<Rigidbody2D>();
        ground = transform.parent.Find("ground");
        slime1 = transform.parent.Find("slimeAI1");
        slime2 = transform.parent.Find("slimeAI2");
        gameManager = transform.parent.GetComponent<GameManagerAgents>();
    }

    private void FixedUpdate()
    {
        //Debug.LogFormat("ball v: {0}", mRigidbody2D.velocity);
        mRigidbody2D.velocity = new Vector2(Mathf.Clamp(mRigidbody2D.velocity.x, -gMaxVX, gMaxVX),
                                            Mathf.Clamp(mRigidbody2D.velocity.y, -gMaxVY, gMaxVY));
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Debug.LogFormat("ball collide {0}", col.collider.name);
        if (col.collider.transform == ground) {
            gameManager.OnRoundEnd(transform.localPosition.x > 0 ? RoundOutcome.LeftScore : RoundOutcome.RightScore);
        } else if (col.collider.transform == slime1) {
            slime1.GetComponent<SlimeAgent>().AddReward(0.01f);
        }  else if (col.collider.transform == slime2) {
            slime2.GetComponent<SlimeAgent>().AddReward(0.01f);
        }
    }

}
