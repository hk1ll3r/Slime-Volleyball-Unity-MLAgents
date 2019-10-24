using UnityEngine;
using UnityEngine.Events;
using MLAgents;

public class SlimeController : MonoBehaviour
{
    [SerializeField] private float mJumpSpeed; // org 18
    [SerializeField] private float mSpeed; // org 8
    [SerializeField] private float mXMax;
    [SerializeField] private float mXMin;
    [SerializeField] private int player;

    private float mGroundY;
    private bool mGrounded;            // Whether or not the player is grounded.
    private Rigidbody2D mRigidbody2D;

    int move, jump;
    private void Awake()
    {
        mRigidbody2D = GetComponent<Rigidbody2D>();
        mGroundY = transform.parent.Find("ground").position.y;
        move = jump = 0;
    }

    // move [0-2] => [-1,1]
    // jump [0,1]
    public void SetAction(int move, int jump) {
        this.move = (move - 1);
        this.jump = jump;
    }

    private void FixedUpdate()
    {
        float vx = move * mSpeed;
        float vy = mRigidbody2D.velocity.y + Physics2D.gravity.y * Time.fixedDeltaTime * 2.4f;

        if (transform.localPosition.y <= 0)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, 0f);
            mRigidbody2D.position = new Vector2(mRigidbody2D.position.x, 0f);
            mGrounded = true;
            vy = 0f;
        }

        if (transform.position.x <= mXMin)
        {
            mRigidbody2D.position = new Vector2(mXMin, mRigidbody2D.position.y);
            if (vx < 0) vx = 0f;
        }
        else if (transform.position.x >= mXMax)
        {
            mRigidbody2D.position = new Vector2(mXMax, mRigidbody2D.position.y);
            if (vx > 0) vx = 0f;
        }

        if (mGrounded && jump == 1)
        {
            // Add a vertical force to the player.
            mGrounded = false;
            vy = mJumpSpeed;
        }

        mRigidbody2D.velocity = new Vector2(vx, vy);
        
    }

}
