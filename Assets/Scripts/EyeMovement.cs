using UnityEngine;
using UnityEngine.Events;

public class EyeMovement : MonoBehaviour
{
    Transform eyeball;
    Transform ball;

    private void Awake()
    {
        eyeball = transform.Find("eyeball");
        ball = transform.parent.parent.Find("ball");
    }

    private void FixedUpdate()
    {        
        eyeball.localPosition = 0.15f * (ball.position - transform.position).normalized;
    }

}
