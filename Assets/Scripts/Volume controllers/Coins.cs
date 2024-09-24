using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }



    private bool collected = false;

    private void OnTriggerEnter2D()
    {
        if (!collected)
        {
            Collect();

        }
    }

    private void Collect()
    {
        collected = true;
        gameObject.SetActive(false);
        EventManager.instance.CoinCollected();
    }

    public void LoadData(PlayerData data)
    {
        data.CoinsCollected.TryGetValue(id, out collected);
        if (collected)
        {
            gameObject.SetActive(false);
        }
    }

    public void SaveData(PlayerData data)
    {
        Debug.Log(id);
        if (data.CoinsCollected.ContainsKey(id))
        {
            data.CoinsCollected.Remove(id);
        }
        data.CoinsCollected.Add(id, collected);
    }
}
