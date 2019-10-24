using UnityEngine;
using UnityEngine.Events;
using MLAgents;

public class SlimeAgent : Agent
{
    [SerializeField] private float mJumpSpeed; // org 18
    [SerializeField] private float mSpeed; // org 8
    [SerializeField] private float mXMax;
    [SerializeField] private float mXMin;
    [SerializeField] private int player;
    [SerializeField] private float gravityMultiplier; // 2.4 for slimes
    GameManagerAgents gameManager;
    Rigidbody2D ground;
    Rigidbody2D ball;
    Rigidbody2D otherSlime;
    Rigidbody2D meSlime;

    private bool mGrounded;            // Whether or not the player is grounded.

    int move, jump;
    public override void InitializeAgent()
    {
        meSlime = GetComponent<Rigidbody2D>();
        
        otherSlime = transform.parent.Find(string.Format("slimeAI{0}", player == 1 ? 2 : 1)).GetComponent<Rigidbody2D>();
        ball = transform.parent.Find("ball").GetComponent<Rigidbody2D>();

        ground = transform.parent.Find("ground").GetComponent<Rigidbody2D>();
        gameManager = transform.parent.GetComponent<GameManagerAgents>();
        
        move = jump = 0;
    }

    int[] movemap = new int[] {0, -1, 1};
    
    public void SetAction(int move, int jump) {
        this.move = movemap[move]; // move [0-2] => [-1,1]
        this.jump = jump; // jump [0,1]
    }

    public Vector2 groundPosition {
        get { return meSlime.position - ground.position; }
        set { meSlime.position = value + ground.position; }
    }

    private Vector2 RefGround(Vector2 v) {
        return v - ground.position;
    }

    private void FixedUpdate()
    {
        //Debug.LogFormat("Agent {0} fixedupdate", player);
        float vx = move * mSpeed;
        float vy = meSlime.velocity.y + Physics2D.gravity.y * Time.fixedDeltaTime * gravityMultiplier;

        if (groundPosition.y <= 0)
        {
            //Debug.LogFormat("Agent {0} y: {1}", player, groundPosition.y);
            groundPosition = new Vector2(groundPosition.x, 0f);
            transform.position = meSlime.position;
            mGrounded = true;
            vy = 0f;
        }

        if (groundPosition.x <= mXMin)
        {
            groundPosition = new Vector2(mXMin, groundPosition.y);
            transform.position = meSlime.position;
            if (vx < 0) vx = 0f;
        }
        else if (groundPosition.x >= mXMax)
        {
            groundPosition = new Vector2(mXMax, groundPosition.y);
            transform.position = meSlime.position;
            if (vx > 0) vx = 0f;
        }

        if (mGrounded && jump == 1)
        {
            // Add a vertical force to the player.
            mGrounded = false;
            vy = mJumpSpeed;
        }

        meSlime.velocity = new Vector2(vx, vy);
        
    }

    public override void AgentReset() {
        // Debug.LogFormat("Agent {0} reset", player);
    }

    private Vector2 InvertX(Vector2 groundVec) {
        return new Vector2(-groundVec.x, groundVec.y);
    }

    // convert the observation to have the agent on the left always
    public override void CollectObservations()
    {
        bool invertX = (player == 2);

        if (invertX) {
            if (gameManager.GameID == 0) Debug.LogFormat("Agent {0} observe ({1}, {2}, {3})", 
                player,
                InvertX(groundPosition),
                InvertX(RefGround(otherSlime.position)), 
                InvertX(RefGround(ball.position))); 
            // positions
            AddVectorObs(InvertX(groundPosition));
            AddVectorObs(InvertX(RefGround(otherSlime.position)));
            AddVectorObs(InvertX(RefGround(ball.position)));

            // velocity
            AddVectorObs(InvertX(meSlime.velocity));
            AddVectorObs(InvertX(otherSlime.velocity));
            AddVectorObs(InvertX(ball.velocity));
        } else {
            if (gameManager.GameID == 0) Debug.LogFormat("Agent {0} observe ({1}, {2}, {3})", 
                player,
                groundPosition, 
                RefGround(otherSlime.position), 
                RefGround(ball.position)); 
            // positions
            AddVectorObs(groundPosition);
            AddVectorObs(RefGround(otherSlime.position));
            AddVectorObs(RefGround(ball.position));

            // velocity
            AddVectorObs(meSlime.velocity);
            AddVectorObs(otherSlime.velocity);
            AddVectorObs(ball.velocity);
        }
        
    }

    // vectorAction size 1 for discrete actions?
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Actions, size = 2
        //Debug.LogFormat("Agent {0} action: {0}, {1}", vectorAction[0], vectorAction[1]);
        // TODO apply action to slime
        SetAction((int)vectorAction[0], (int)vectorAction[1]);
        AddReward(-0.001f);

        // Reward
        /*if (_roundEnded)
        {
            _roundEnded = false;
            float reward = (_lastScorer == player) ? 1 : -1;
            Debug.LogFormat("Agent {0} done (reward: {1})", player, reward);
            SetReward(reward);
            Done();
        }*/

    }

}
