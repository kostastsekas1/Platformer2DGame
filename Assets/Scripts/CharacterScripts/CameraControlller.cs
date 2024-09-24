using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlller : MonoBehaviour
{
    public Camera playercamera;
    public Transform Player;
    public Vector2 Margin , Smooth;

    public BoxCollider2D CameraBox;
    private Vector3 min, max;


    public  bool CameraIsFollowing { get;set; }

    public void Start()
    {
        min = CameraBox.bounds.min;
        max = CameraBox.bounds.max;
        CameraIsFollowing = true;
    }

    private void Update()
    {
        var x = transform.position.x;
        var y = transform.position.y;

        if (CameraIsFollowing)
        {
            if (Mathf.Abs(x-Player.position.x)> Margin.x)
            {
                x = Mathf.Lerp(x, Player.position.x, Smooth.x * Time.deltaTime);
            }

            if ( Mathf.Abs(y- Player.position.y)> Margin.y)
            {
                y = Mathf.Lerp(y,Player.position.y,Smooth.y * Time.deltaTime);
            }
        }
        var halfcamerawidth = playercamera.orthographicSize * ((float)Screen.width / Screen.height);
        
        x =  Mathf.Clamp(x,min.x + halfcamerawidth, max.x - halfcamerawidth);
        y = Mathf.Clamp(y, min.y + playercamera.orthographicSize, max.y - playercamera.orthographicSize);

        transform.position = new Vector3 (x, y,transform.position.z);
    }
}
