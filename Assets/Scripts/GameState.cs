using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    private Vector2 _slime1Position;
    private Vector2 _slime1Velocity;

    private Vector2 _slime2Position;
    private Vector2 _slime2Velocity;

    private Vector2 _ballPosition;
    private Vector2 _ballVelocity;

    public GameState(
        Vector2 slime1Position,
        Vector2 slime1Velocity,
        Vector2 slime2Position,
        Vector2 slime2Velocity,
        Vector2 ballPosition,
        Vector2 ballVelocity)
    {
        _slime1Position = slime1Position;
        _slime1Velocity = slime1Velocity;

        _slime2Position = slime2Position;
        _slime2Velocity = slime2Velocity;

        _ballPosition = ballPosition;
        _ballVelocity = ballVelocity;
    }
}
