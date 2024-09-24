using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerState
{
    public bool collisionRight {  get; set; }
    public bool collisionLeft { get; set; }
    public bool collisionUp { get; set; }
    public bool collisionDown { get; set; }
    public bool GoingDownSlope {  get; set; }
    public bool GoingUpSlope { get; set; }
    public bool IsGrounded { get { return collisionDown; }}
    public float SlopeDegree { get; set; }
    public bool IsColliding { get { return collisionDown || collisionLeft || collisionRight || collisionUp; } }

    public void Reset()
    {
        collisionRight = collisionLeft = collisionUp = collisionDown = GoingDownSlope = GoingUpSlope = false;

        SlopeDegree = 0;
    }

    public override string ToString()
    {
        return string.Format("(Controller: r: {0} l: {1} u:{2} d: {3} downSlope: {4} upslope: {5} slopedegree: {6})",
            collisionRight,collisionLeft,collisionUp, collisionDown , GoingDownSlope, GoingUpSlope, SlopeDegree);
    }
}
