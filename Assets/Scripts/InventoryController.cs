using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System;
using UnityEngine.Networking;


public class InventoryController : MonoBehaviour
{
    // Load the Json file from the URL
    //private string InvData1 = "http://balak.it/testdata.json";

    private Inventory m_currentInventory;
    private float RANDOM_FRACTION_PER_FRAME = 0.01f;

    public Inventory CurrentInventory
    {
        get { return m_currentInventory; }
    }

    public static InventoryController Instance;

    private IEnumerator Start()
    {
        Instance = this;
        yield return GetInventoryFromWeb();
    }

    public Crate GetCrateForFruit(string inputFruit)
    {
        if (m_currentInventory == null || m_currentInventory.Crates == null)
        {
            return null;
        }

        for (int i = 0; i < m_currentInventory.Crates.Length; i++)
        {
            if (inputFruit == m_currentInventory.Crates[i].ItemName)
            {
                return m_currentInventory.Crates[i];
            }
        }

        return null;
    }

    IEnumerator GetInventoryFromWeb()
    {
        UnityWebRequest getData = UnityWebRequest.Get("http://balak.it/testdata.json");
        yield return getData.SendWebRequest();

        if (getData.isNetworkError || getData.isHttpError)
        {
            Debug.Log(getData.error);
        }
        else
        {
            // Show results as text
            var jsonData = getData.downloadHandler.text;

            m_currentInventory = JsonUtility.FromJson<Inventory>(jsonData);
        }

    }

    public void Update()
    {
        RandomlyDecrementFruits();
    }

    private void RandomlyDecrementFruits()
    {
        if (m_currentInventory != null && m_currentInventory.Crates != null)
        {

            for (int i = 0; i < m_currentInventory.Crates.Length; i++)
            {
                if (UnityEngine.Random.Range(0f, 1f) < RANDOM_FRACTION_PER_FRAME)
                {

                    if (m_currentInventory.Crates[i].Quantity > 0)
                    {
                        m_currentInventory.Crates[i].Quantity -= 1;
                    }
                }
            }
        }
    }
}

[Serializable]
public class Inventory
{
    public Crate[] Crates;
}

[Serializable]
public class Crate
{
    public int CrateID = 0;
    public string ItemName = "a";
    public int Quantity = 1;
    public string LastStockDate = "b";
    public string LastStockTime = "default";
    public string ExpirationDate = "default";
    public string ExpirationTime = "default";

    public DateTime LastStockDatetime
    {
        get { return ParseDateTime(LastStockDate, LastStockTime); }
    }

    public DateTime ExpirationDatetime
    {
        get { return ParseDateTime(ExpirationDate, ExpirationTime); }
    }

    private DateTime ParseDateTime(string date, string time)
    {
        return Convert.ToDateTime(date + " " + time);
    }

}