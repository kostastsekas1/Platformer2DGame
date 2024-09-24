using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Able to modify it if there is a public field on a monobehaviour script that this class is attached to

[Serializable]
public class ControllerParams 
{
     public enum CanJump
    {
        CanJumpOnGround,
        CanJumpAnywhere,
        CantJump
    }

    public Vector2 MaxSpeedForce= new Vector2(float.MaxValue, float.MaxValue);

    [Range(0, 90)]
    public float SlopeDegreeLimit = 30;

    public float gravity = -30f;

    public CanJump JumpPermission;

    public float jumpForce = 15;

    public float coyotetime = 0.2f;
   
}
