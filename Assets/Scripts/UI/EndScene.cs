using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour, IDataPersistence
{
    public TextMeshProUGUI Deaths;
    public TextMeshProUGUI Score;
    public TextMeshProUGUI CoinsCollected;
    public TextMeshProUGUI TotalTime;
    private int deathcount;
    private int scorecount;
    private int totalcoins;
    private int coinscollected;
    private float timeSpent;


    public void LoadData(PlayerData data)
    {
        scorecount = data.score;
        deathcount = data.deaths;
        totalcoins = data.CoinsCollected.Count;
        timeSpent = data.timespentonLevel;

        foreach (KeyValuePair<string, bool> pair in data.CoinsCollected)
        {
            if (pair.Value)
            {
                coinscollected++;
            }
        }

    }

    public void SaveData(PlayerData data)
    {
        return;
    }

    void Update()
    {
        Score.text = "Total Score: " + scorecount;

        TotalTime.text = string.Format("Total Time: {0}:{1:00}", Mathf.FloorToInt(timeSpent / 60), Mathf.Round(timeSpent % 60));

        Deaths.text = "Total Deaths: " + deathcount;

        CoinsCollected.text = "Coins Collected: " + coinscollected + "/" + totalcoins;
        
    }


    public void BackToMainMenu()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 2);
    }
}
