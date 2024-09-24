using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinProgress : MonoBehaviour, IDataPersistence
{
    private int totalCoins;
    private int collectedCounter;
    public TextMeshProUGUI coinsCollectedText;

    public void LoadData(PlayerData data)
    {

        foreach (KeyValuePair<string, bool> pair in data.CoinsCollected)
        {
            if (pair.Value)
            {
                collectedCounter++;
            }
        }


    }

    private void Awake()
    {
       totalCoins = FindObjectsOfType<Coins>().Length;
       
    }


    public void SaveData(PlayerData data)
    {
        return;
    }

    private void Start()
    {
        EventManager.instance.onCoinCollected += OnCoinCollected;
 
    }
    private void OnCoinCollected()
    {
        collectedCounter++;
    }

    private void Update()
    {
        coinsCollectedText.text = collectedCounter + " / " + totalCoins;
    }

}
