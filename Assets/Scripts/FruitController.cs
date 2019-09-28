using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitController : MonoBehaviour
{
    [SerializeField]
    private List<Fruit> m_fruits;

    private static FruitController s_fruitController;

    public static FruitController Instance
    {
        get { return s_fruitController; }
    }

    private void Start()
    {
        s_fruitController = this;
    }

    public Fruit GetFruit(string inputString)
    {
        for(int i = 0; i < m_fruits.Count; i++)
        {
            if(inputString == m_fruits[i].DatabaseName)
            {
                return m_fruits[i];
            }
        }

        return null;
    }
}

[System.Serializable]
public class Fruit
{
    public string DatabaseName;
    public string DisplayName;
    public Texture DisplayImage;
}