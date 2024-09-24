using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterController2D : MonoBehaviour, IDataPersistence
{

    #region Variables
    private const float hitboxWidth = 0.02f;
    private const int HorizontalRays = 8; // for y axis
    private const int VerticalRays = 4; //for x axis
    private static readonly float SlopeLimitTan = Mathf.Tan(75f * Mathf.Deg2Rad);
    public LayerMask platformsMask;

    public ControllerParams controllerParams;
    public ControllerParams Parameters { get { return OverrideControllerParams ?? controllerParams; } }
    public bool HandleCollisions { get; set; }
    public ControllerState state { get; private set; }
    public Vector2 SpeedForce { get { return _SpeedForce; } }
    public Vector2 RespawnPoint;
    private bool isdead = false;
    public float coyotetimecounter;

    public bool CanJump {
        get
        {
            if (Parameters.JumpPermission == ControllerParams.CanJump.CanJumpAnywhere) 
            {
                return jumpfrequency <= 0;
            }
            if (Parameters.JumpPermission == ControllerParams.CanJump.CanJumpOnGround)
            {
                if ( state.IsGrounded)
                {
                    coyotetimecounter = Parameters.coyotetime;
                }
                else
                {
                    coyotetimecounter -= Time.deltaTime;
                }
                return coyotetimecounter > 0;
            }

            return false; }
    }

    private Vector2 _SpeedForce;

    private Vector3 rayTopLeft, rayBottomRight , rayBottomLeft;

    private Transform _transform;
    private Vector3 _localScale;
    private BoxCollider2D _boxCollider;
    private float jumpfrequency;
    private float DistanceBetweenVerticalRays, DistanceBetweenHorizontalRays;
    private ControllerParams OverrideControllerParams;
    #endregion


    #region save and  load 
    public void LoadData(PlayerData data)
    {
        Vector2 playerpositionvec = new Vector2(data.playerposition[0], data.playerposition[1]);

        this.transform.position = playerpositionvec;
        RespawnPoint = new Vector2(data.RespawnPoint[0], data.RespawnPoint[1]);
    }

    public void SaveData(PlayerData data)
    {

        data.playerposition[0] = this.transform.position.x;
        data.playerposition[1] = this.transform.position.y;
        
        data.RespawnPoint[0] = RespawnPoint.x;
        data.RespawnPoint[1] = RespawnPoint.y;

        

        data.saveDate = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();


    }


    #endregion


    public void Awake()

    {
        HandleCollisions= true;
        state = new ControllerState();
        _transform = transform;
        _localScale = transform.localScale;
        _boxCollider = GetComponent<BoxCollider2D>();
        

        DistanceBetweenHorizontalRays = (_boxCollider.size.x *Mathf.Abs(_localScale.x)-2*hitboxWidth)/(VerticalRays-1);
        DistanceBetweenVerticalRays   = (_boxCollider.size.y *Mathf.Abs(_localScale.y)-2*hitboxWidth)/(HorizontalRays-1) ;
         
    }

    #region handling forces
    public void  addForce (Vector2 force)
    {
        _SpeedForce = force;

    }

    public void setForce (Vector2 force)
    {
        _SpeedForce += force;
    }

    public void SetHorizontalForce (float x)
    {
        _SpeedForce.x = x;
    }

    public void SetVerticalForce (float y)
    {
        _SpeedForce.y = y;
    }
    
    public void jump()
    {
        addForce(new Vector2(_SpeedForce.x, Parameters.jumpForce  ));
        jumpfrequency = Parameters.coyotetime;
        coyotetimecounter = 0;
    }

    public void LateUpdate()
    {
        if (isdead)
        {
            return;
        }

        if (_transform.position.y < -60f)
        {
            _transform.position = RespawnPoint;
        }

        jumpfrequency -=Time.deltaTime;
        _SpeedForce.y += Parameters.gravity * Time.deltaTime;
        Move(SpeedForce*Time.deltaTime);
    }

    private void Move(Vector2 deltamovement)
    {
        var Grounded = state.collisionDown;
        state.Reset();

        if (HandleCollisions) 
        {

            CalculateRayOrigins();
            
            if(deltamovement.y<0 &&  Grounded)
            {
                HandleSlopeVertically(ref deltamovement);
            }

            if (Mathf.Abs(deltamovement.x)> 0.001f)
            {
                MoveHorizontally(ref deltamovement);
            }


            MoveVertically(ref deltamovement);

            correcthorizontalplacement(ref deltamovement, true);
            correcthorizontalplacement(ref deltamovement, false);
        }

        _transform.Translate(deltamovement, Space.World);   



        if(Time.deltaTime > 0)
        {
            _SpeedForce = deltamovement/Time.deltaTime;

        }
        _SpeedForce.x = Mathf.Min(_SpeedForce.x, Parameters.MaxSpeedForce.x);
        _SpeedForce.y = Mathf.Min(_SpeedForce.y, Parameters.MaxSpeedForce.y);

        if(state.GoingUpSlope)
        {
            _SpeedForce.y = 0;
        }

    }
    #endregion

    #region calculating rays from inside hitbox to end of hitbox and out of hitbox for collisions

    private void CalculateRayOrigins()
    {
        var _size = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.x), _boxCollider.size.y * Mathf.Abs(_localScale.y)) /2;

        var hitboxcenter = new Vector2(_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);
        rayTopLeft = _transform.position + new Vector3(hitboxcenter.x - _size.x + hitboxWidth, hitboxcenter.y + _size.y - hitboxWidth);

        rayBottomRight = _transform.position + new Vector3(hitboxcenter.x + _size.x - hitboxWidth, hitboxcenter.y -_size.y +hitboxWidth);

        rayBottomLeft = _transform.position + new Vector3(hitboxcenter.x -_size.x + hitboxWidth, hitboxcenter.y - _size.y + hitboxWidth);
    }

    private void  correcthorizontalplacement (ref Vector2 deltamovement,bool isright)
    {
        var halfwidth = (_boxCollider.size.x * _localScale.x)/2;
        


        var rayorigin = isright ?  rayBottomRight : rayBottomLeft;


        if (isright)
        {
            rayorigin.x -= (halfwidth - hitboxWidth);
        }
        else
        {
            rayorigin.x += (halfwidth - hitboxWidth);
        }
        
        var raydirection = isright ? Vector2.right : Vector2.left;

        var offset = 0f;
        for (var i =1; i < HorizontalRays-1;i++)
        {

            var rayvector = new Vector2(deltamovement.x +rayorigin.x, deltamovement.y + rayorigin.y  + (i * DistanceBetweenVerticalRays));

            Debug.DrawRay(rayvector, raydirection * halfwidth, isright ? Color.red : Color.blue) ;

            var raycasthit = Physics2D.Raycast(rayvector, raydirection, halfwidth, platformsMask);

            if (!raycasthit)
            {
                continue;
            }
            offset = isright ? ((raycasthit.point.x - _transform.position.x)-halfwidth) : (halfwidth - (_transform.position.x - raycasthit.point.x));

        }
        deltamovement.x += offset;
    }

    #endregion

    #region Move horizontally and vertically

    private void MoveHorizontally(ref Vector2  deltamovement)
    {
        var GoingRight = deltamovement.x > 0;

        var rayDistance = Mathf.Abs(deltamovement.x) + hitboxWidth;

        var rayDirection = GoingRight ?Vector2.right:Vector2.left;

        var rayOrigin = GoingRight? rayBottomRight : rayBottomLeft;
        
        for (int i = 0; i < HorizontalRays; i++)
        {
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y+ (i*DistanceBetweenVerticalRays));

            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);



            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance,platformsMask);
            
            if(!raycastHit)
            {
                continue;
            }

            if (i==0 && HandleSlopeHorizontally(ref deltamovement, Vector2.Angle(raycastHit.normal, Vector2.up), GoingRight))
            {
                break;
            }

            deltamovement.x = raycastHit.point.x - rayVector.x;

            rayDistance = Mathf.Abs(deltamovement.x);
            
            if (GoingRight  )
            {
                deltamovement.x -= hitboxWidth;
                state.collisionRight = true;
            }
            else
            {
                deltamovement.x+= hitboxWidth; 
                state.collisionLeft = true;

            }

            if (rayDistance < hitboxWidth+ 0.0001f)
            {
                break;
            }
        }

    }

    private void MoveVertically(ref Vector2 deltamovement)
    {
        var GoingUp = deltamovement.y > 0;
        var raydistance = Mathf.Abs(deltamovement.y) + hitboxWidth;
        var raydirection =  GoingUp? Vector2.up : Vector2.down;
        var rayorigin = GoingUp ? rayTopLeft : rayBottomLeft;

        rayorigin.x += deltamovement.x;

        for(int i = 0; i < VerticalRays; i++) 
        {
            var rayvector = new Vector2(rayorigin.x + (i * DistanceBetweenHorizontalRays), rayorigin.y);
            Debug.DrawRay(rayvector, raydirection * raydistance *2, Color.red);
            var raycasthit = Physics2D.Raycast(rayvector, raydirection, raydistance, platformsMask);

            if (!raycasthit)
            {
               
                continue;
            }
            
           
        
            deltamovement.y = raycasthit.point.y - rayvector.y;
            raydistance = Mathf.Abs(deltamovement.y);

            if (GoingUp)
            {
                deltamovement.y -= hitboxWidth;

                state.collisionUp = true;
            }
            else
            {
                deltamovement.y += hitboxWidth;


                state.collisionDown = true;
            }

            if (!GoingUp && deltamovement.y > 0.0001f)
            {
                state.GoingUpSlope = true;
            }

            if (raydistance < hitboxWidth + 0.0001f)
            {
                break;
            }
        }

    }
    #endregion


    #region  handle slopes horizontally and vertically

    //going down
    private void HandleSlopeVertically(ref Vector2 deltamovement) 
    {

        var center = (rayBottomLeft.x +  rayBottomRight.x) / 2;
        var direction = Vector2.down;

        var slopedistance = SlopeLimitTan*(rayBottomRight.x - center);
        var slopeRayvector = new Vector2(center, rayBottomLeft.y);
        Debug.DrawRay(slopeRayvector, direction * slopedistance, Color.yellow);

        var raycasthit = Physics2D.Raycast(slopeRayvector, direction, slopedistance, platformsMask);

        if (!raycasthit)
        {
            return;
        }

        var ismovingdown = Mathf.Sign(raycasthit.normal.x) == Mathf.Sign(deltamovement.x);
        Debug.Log(raycasthit.normal.x);
        if(!ismovingdown)
        {
            return;
        }

        var angle = Vector2.Angle(raycasthit.normal, Vector2.up);
       

        if (Mathf.Abs(angle)< 0.0001f)
        {
            return;
        }

        state.GoingDownSlope = true;
        state.SlopeDegree = angle;
        deltamovement.y=raycasthit.point.y - slopeRayvector.y;
    }

    //going up
    private bool HandleSlopeHorizontally(ref Vector2 deltamovement,float angle, bool isgoingRight) 
    {

        if (Mathf.RoundToInt(angle) == 90)
        {
            return false;
        }

        if (angle > Parameters.SlopeDegreeLimit)
        {
            deltamovement.x= 0;
           
            return true;
        }

        if (deltamovement.y > 0.07f)
        {
            return true;
        }




        deltamovement.y = Mathf.Abs(Mathf.Tan(angle*Mathf.Deg2Rad)*deltamovement.x);

        state.GoingUpSlope = true;
        state.collisionDown = true;
        return true;
    }

    #endregion


    #region handle trigger colliders 
    public void OnTriggerEnter2D(Collider2D volume)
    {
        var parameters = volume.gameObject.GetComponent<ControllerVolumeTrigger>();
        if (volume.gameObject.tag.Equals("checkpoint"))
        {
            RespawnPoint = volume.transform.position;
        }
        if (volume.gameObject.tag.Equals("Finish"))
        {
            StartCoroutine(HandleFinish());
        }

        if (volume.gameObject.tag.Equals("DeathMechanism"))
        {
            StartCoroutine(HandleDeath());
        }

        if (parameters == null)
        {
            return;
        }

        OverrideControllerParams = parameters.controllerParams;

        if (parameters.Fan)
        {
            _SpeedForce.y += parameters.Boyancyfactor * OverrideControllerParams.gravity * Time.deltaTime;
        }
    }
    public void OnTriggerExit2D(Collider2D volume)
    {
        var parameters = volume.gameObject.GetComponent<ControllerVolumeTrigger>();

        if (parameters == null)
        {
            return;
        }
        OverrideControllerParams =null;
    }

    #endregion
    #region handle death
    IEnumerator HandleDeath()
    {
        HandleCollisions = false;
        _boxCollider.enabled = false;

        isdead = true;
        EventManager.instance.DeathsCounter();
        OverrideControllerParams = null;
        yield return new WaitForSeconds(0.4f);
        Respawn();
    }
    private void Respawn()
    {
        isdead = false;
        HandleCollisions = true;
        _boxCollider.enabled = true;
        _transform.position = RespawnPoint;
    }
    #endregion

    IEnumerator HandleFinish()
    {
        yield return new WaitForSeconds(2f);
        DataPersistence.instance.SaveGame();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
}