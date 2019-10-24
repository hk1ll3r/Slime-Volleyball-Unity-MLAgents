using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMapper : MonoBehaviour
{
    private string mHAxis;
    private string mVAxis;

    [SerializeField] private int player;

    private float move;
    private int jump;

    private SlimeAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        mHAxis = string.Format("Horizontal{0}", player);
        mVAxis = string.Format("Vertical{0}", player);
        agent = GetComponent<SlimeAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float move = Input.GetAxis(mHAxis);
        bool jump = Input.GetButton(mVAxis);
        agent.SetAction(Mathf.RoundToInt(move), jump ? 1 : 0);
    }
}
