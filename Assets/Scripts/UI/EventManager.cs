using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More Than one manager found");
        }
        instance = this;
    }

    public event Action onCoinCollected;
    public void CoinCollected()
    {
        if (onCoinCollected != null)
        {
            onCoinCollected();
        }
    }

    public event Action Death;
    public void DeathsCounter()
    {
        if (Death != null)
        {
            Death();
        }
    }

}
