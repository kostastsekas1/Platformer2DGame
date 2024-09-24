using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[System.Serializable]
public class PlayerData
{
    public long lastUpdated;
    public float[] playerposition;
    public float[] RespawnPoint;
    public string saveDate;
    public SerialisableDictionary<string, bool>  CoinsCollected;
    public int deaths;
    public float timespentonLevel;
    public int score;
    public PlayerData()
    {
       

        playerposition= new float[2];

        playerposition[0] = 5f;
        playerposition[1] = 5f;

        RespawnPoint = new float[2];

        RespawnPoint[0] = 5f;
        RespawnPoint[1] = 5f;

        saveDate = "";
        CoinsCollected = new SerialisableDictionary<string, bool>();
        timespentonLevel = 0;
        deaths = 0;
        score = 1000;
    }
    
    public  int PercentageOfCoinsCollected()
    {
        int coins = 0;

        foreach(bool collected in CoinsCollected.Values) 
        {
            if (collected)
            {
                coins++;
            }
        }

        int percentagecomplete = -1;
        if(CoinsCollected.Count !=0 )
        {
            percentagecomplete = (coins * 100 / CoinsCollected.Count);
        }
      

        return percentagecomplete;
    }
   
   
}
