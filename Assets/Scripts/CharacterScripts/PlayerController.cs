using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool facingright;
    private CharacterController2D characterController;
    private float horizontalspeed;
    private  ControllerState state;
    public Animator animator;

    public float maxspeed=8;
    public float SpeedAclOnGround= 10f;
    public float SpeedAclOnAir = 5f;

    public void Start()
    {
        characterController = GetComponent<CharacterController2D>();
        facingright = transform.localScale.x > 0;
    }

    public void Update()
    {
        InputHandle();
        var movement = characterController.state.IsGrounded?SpeedAclOnGround: SpeedAclOnAir;
        characterController.SetHorizontalForce(Mathf.Lerp(characterController.SpeedForce.x,horizontalspeed*maxspeed,Time.deltaTime*movement));

        animator.SetFloat("speed", Mathf.Abs(horizontalspeed));
        animator.SetBool("IsJumping", !characterController.CanJump);
    }

    private void InputHandle()
    {
        if (Input.GetKey(KeyCode.D))
        {
            horizontalspeed = 1;
            if(!facingright)
            {
                Flip();
            }
        }else if (Input.GetKey(KeyCode.A)) 
        {
            horizontalspeed = -1;
            if (facingright)
            {
                Flip();
            }
        }
        else
        {
            horizontalspeed = 0;
        }

       if (characterController.CanJump && (Input.GetKeyDown(KeyCode.W)|| Input.GetKeyDown(KeyCode.Space)) ) 
        {

            characterController.jump();
        }

    }

    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,transform.localScale.z);
        facingright= transform.localScale.x > 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent =null;
        }
    }

}
