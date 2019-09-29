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
    private float RANDOM_FRACTION_PER_FRAME = 0.0005f;
    private Coroutine m_decrementFruitCoroutine = null;

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

    public void RefreshAllData()
    {
        StartCoroutine(GetInventoryFromWeb());
    }

    public Crate GetCrateForFruit(string inputFruit)
    {
        if (m_currentInventory == null || m_currentInventory.Crates == null)
        {
            return null;
        }

        Crate currentCrate = null;

        for (int i = 0; i < m_currentInventory.Crates.Length; i++)
        {
            if (inputFruit == m_currentInventory.Crates[i].item_name)
            {
                if(currentCrate == null || currentCrate.id < m_currentInventory.Crates[i].id)
                {
                    currentCrate = m_currentInventory.Crates[i];
                }
            }
        }

        return currentCrate;
    }

    IEnumerator GetInventoryFromWeb()
    {
        UnityWebRequest getData = UnityWebRequest.Get("http://192.168.2.253:8000/food/api/food/?format=json");
        yield return getData.SendWebRequest();

        if (getData.isNetworkError || getData.isHttpError)
        {
            Debug.Log(getData.error);
        }
        else
        {
            // Show results as text
            var jsonData = getData.downloadHandler.text;

            m_currentInventory = JsonUtility.FromJson<Inventory>("{\"Crates\":"+jsonData+"}");
        }

    }

    public void Update()
    {
        //RandomlyDecrementFruits();
    }

    private void RandomlyDecrementFruits()
    {
        if (m_currentInventory != null && m_currentInventory.Crates != null)
        {

            for (int i = 0; i < m_currentInventory.Crates.Length; i++)
            {
                if (UnityEngine.Random.Range(0f, 1f) < RANDOM_FRACTION_PER_FRAME && m_decrementFruitCoroutine == null)
                {

                    if (m_currentInventory.Crates[i].quantity > 0)
                    {
                        m_decrementFruitCoroutine = StartCoroutine(DecrementAFruit(m_currentInventory.Crates[i].item_name));
                    }
                }
            }
        }

        
    }

    private IEnumerator DecrementAFruit(string fruitName)
    {
        var foundCrate = GetCrateForFruit(fruitName);

        WWWForm newData = new WWWForm();
        newData.AddField("crate_id", foundCrate.crate_id);
        newData.AddField("item_name", foundCrate.item_name);
        newData.AddField("quantity", (int)Mathf.Clamp(foundCrate.quantity-1, 0, float.MaxValue));

        //expiration time should be 5hr+old expiration time
        var expirationTimeDelta = foundCrate.ExpirationDatetime - (DateTime.UtcNow.AddSeconds(13));

        newData.AddField("expiration_hours", (int)expirationTimeDelta.TotalHours); // somehow keep the same
        newData.AddField("expiration_minutes", expirationTimeDelta.Minutes);


        UnityWebRequest getData = UnityWebRequest.Post("http://192.168.2.253:8000/food/api/food/?format=api", newData);
        yield return getData.SendWebRequest();

        if (getData.isNetworkError || getData.isHttpError)
        {
            Debug.Log(getData.error);
        }
        else
        {
            Debug.Log("Success updating inventory");
        }

        // then update the data
        m_decrementFruitCoroutine = null;
        InventoryController.Instance.RefreshAllData();
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
    public int id = 0;
    public int crate_id = 0;
    public string item_name = "a";
    public int quantity = 1;
    public string last_stock_datetime = "default";
    public string expiration_datetime = "default";

    public DateTime LastStockDatetime
    {
        get { return Convert.ToDateTime(last_stock_datetime); }
    }

    public DateTime ExpirationDatetime
    {
        get { return Convert.ToDateTime(expiration_datetime); } // RPB: the -5f hours is to account for a db bug
    }

}