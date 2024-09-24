using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    public enum Follow
    {
        MoveTowards,
        Lerp,
        Teleport
    }
    public Follow Type = Follow.MoveTowards;
    public Paths Path;
    public float Speed = 1f;
    public float MaxDistanceToPoint = 0.1f;
    private bool teleporteractived = false;
    private IEnumerator<Transform> Point;

    public void Start()
    {
        if (Path == null)
        {
            Debug.LogError("Path not present");
            return;
        }
        Point = Path.GetPathNext();
        Point.MoveNext();

        if (Point.Current ==null)
        {
            return;
        }
        transform.position = Point.Current.position;
    }

    private void Update()
    {
        movePlatformsdoors();
    }

    public void movePlatformsdoors()
    {
        if (Point == null || Point.Current == null)
        {
            return;
        }

        if (Type == Follow.MoveTowards)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                                                     Point.Current.position,
                                                     Time.deltaTime * Speed);
        }
        else if (Type == Follow.Lerp)
        {
            transform.position = Vector3.Lerp(transform.position,
                                                    Point.Current.position,
                                                    Time.deltaTime * Speed);
            Debug.Log(Point.Current.position);
        }
        else if (Type == Follow.Teleport && !teleporteractived)
        {
                    StartCoroutine(teleportbuffer()); 
        }

        float distancesqr = (transform.position - Point.Current.position).sqrMagnitude;
        if (distancesqr < Mathf.Pow(MaxDistanceToPoint, 2))
        {
            Point.MoveNext();
        }
    }

    IEnumerator teleportbuffer()
    {
        teleporteractived = true;
        yield return new WaitForSeconds(5f);
        transform.position = Point.Current.position;
        teleporteractived = false; 
    }
}

