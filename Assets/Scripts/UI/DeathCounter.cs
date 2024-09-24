using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathCounter : MonoBehaviour, IDataPersistence
{
    private int Deaths;
    public TextMeshProUGUI DeathText;

    public void LoadData(PlayerData data)
    {
        Deaths = data.deaths;
    }

    public void SaveData(PlayerData data)
    {
        data.deaths = Deaths;
    }

    private void Start()
    {
        EventManager.instance.Death += OnDeath;

    }
    private void OnDeath()
    {
        Deaths++;
    }

    private void Update()
    {
        DeathText.text = "Deaths: " + Deaths;
    }
}
