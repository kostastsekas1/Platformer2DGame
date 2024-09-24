using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Scoring : MonoBehaviour, IDataPersistence
{
    public int timeDeductionRate = 3;

    private float currentTime;
    private int coinsCollected;
    private int finalScore;
    private int deaths=0;

    public TextMeshProUGUI Score;

    private void Start()
    {
        EventManager.instance.onCoinCollected += OnCoinCollected;
        EventManager.instance.Death += OnDeath;

    }
    private void OnCoinCollected()
    {
        coinsCollected++;
    }
    private void OnDeath()
    {
        deaths++;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (Score != null ) {
            finalScore = Mathf.Max(0, (1000 - Mathf.RoundToInt(currentTime) * timeDeductionRate));
            if (coinsCollected != 0)
            {
                finalScore = Mathf.RoundToInt( (finalScore* coinsCollected));
            }
            if (deaths != 0) 
            {
                finalScore = Mathf.RoundToInt(finalScore / (Mathf.Exp( deaths)));
            }
            Score.text = "Score: " + finalScore;
        }
    }

    public void LoadData(PlayerData data)
    {
        currentTime = data.timespentonLevel;
        deaths += data.deaths;
        finalScore = data.score;
        foreach (KeyValuePair<string, bool> pair in data.CoinsCollected)
        {
            if (pair.Value)
            {
                coinsCollected++;
            }
        }
    }

    public void SaveData(PlayerData data)
    {
        data.timespentonLevel = currentTime;
        data.score = finalScore;
    }
}
