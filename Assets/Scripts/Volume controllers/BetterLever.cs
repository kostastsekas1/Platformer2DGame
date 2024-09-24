using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BetterLever : MonoBehaviour
{
    private bool inTrigger = false;
    public bool timedDoor = false;
    public float timer = 2;
    public Transform[] Platforms;
    public Paths[] Paths;
    public float Speed = 1f;
    public float MaxDistanceToPoint = 0.1f;

    private IEnumerator<Transform> Point;
    private List<IEnumerator<Transform>> Points = new List<IEnumerator<Transform>>();
    public void Start()
    {
        if (Platforms.Length != Paths.Length)
        {
            Debug.LogError("Platforms and Paths lengths do not match.");
            return;
        }

        for (int i = 0; i < Platforms.Length; i++)
        {
            var path = Paths[i];
            if (path == null)
            {
                Debug.LogError($"Path at index {i} is not present.");
                continue;
            }

            var point = path.GetPathNext();
            point.MoveNext();
            if (point.Current == null)
            {
                return;
            }
            Points.Add(point);
            Platforms[i].position = point.Current.position;
        }

      
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            inTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            inTrigger = false;
        }
    }

    private void Update()
    {
        for (int i = 0; i < Points.Count; i++)
        {
            var point = Points[i];
            if (point == null || point.Current == null)
                continue;

            var platform = Platforms[i];

            platform.position = Vector3.MoveTowards(platform.position, point.Current.position, Time.deltaTime * Speed);
            float distancesqr = (platform.position - point.Current.position).sqrMagnitude;

            if (distancesqr < Mathf.Pow(MaxDistanceToPoint, 2))
            {
                if (inTrigger && Input.GetKeyDown(KeyCode.E))
                {
                    point.MoveNext();
                    if(timedDoor)
                    {
                        StartCoroutine(CloseDoorTimer(i));
                        
                    }
                }
            }
        }
    }
    IEnumerator CloseDoorTimer(int i)
    {
        yield return new WaitForSeconds(timer);
        Points[i].MoveNext();
    }
}




