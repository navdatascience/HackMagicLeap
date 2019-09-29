using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttentionNode : MonoBehaviour
{
    private static List<AttentionNode> s_attentionNodes = new List<AttentionNode>();

    private const float TOLERANCE_RED = 0.75f;
    private const float TOLERANCE_YELLOW = 0.0f;
    private const float CULL_DISTANCE = 20f;

    private const int HEALTHY_STOCK_QUANTITY = 20;

    [SerializeField]
    private Text m_text = null;

    [SerializeField]
    private RawImage m_template = null;

    [SerializeField]
    private Texture m_redTemplate = null;


    [SerializeField]
    private Texture m_yellowTemplate = null;


    [SerializeField]
    private Texture m_greenTemplate = null;


    [SerializeField]
    private Texture m_yellowCullTemplate = null;


    [SerializeField]
    private Texture m_greenCullTemplate = null;


    [SerializeField] //lol
    private Texture m_redCullTemplate = null;


    [SerializeField]
    private GameObject m_objectsCanCull = null;

    [SerializeField]
    private Text m_displayName = null;

    [SerializeField]
    private RawImage m_displayImage = null;

    [SerializeField]
    private Text m_quantityLabel = null;

    [SerializeField]
    private string m_fruitString = "FILL ME IN";

    [SerializeField]
    private Text m_timeToExpirationLabel = null;

    [SerializeField]
    private Text m_timeToExpirationLabel2 = null;

    private bool m_previousShouldCull = false;
    private Crate m_cachedCrate = null;

    private float m_timeUrgencyForFrame = 0f;

    public float Urgency
    {
        get
        {
            if (m_cachedCrate != null)
            {
                var quantityUrgency = Mathf.Clamp01(((float)HEALTHY_STOCK_QUANTITY - (float)m_cachedCrate.Quantity) / (float)HEALTHY_STOCK_QUANTITY);

                

                //Debug.LogError($"time till expiration: {timeUntilExpiration.TotalMinutes}");

                if(m_timeUrgencyForFrame > quantityUrgency)
                {
                    return m_timeUrgencyForFrame;
                }
                else
                {
                    return quantityUrgency;
                }
            }
            else
            {
                return 0f;
            }

        }
    }

    public static List<AttentionNode> AttentionNodes
    {
        get { return s_attentionNodes; }
    }

    private void Start()
    {
        s_attentionNodes.Add(this);

        UpdateView();
    }

    private void Update()
    {
        var shouldCull = (Vector3.Distance(this.transform.position, PlayerController.Instance.transform.position) > CULL_DISTANCE);
        var crate = InventoryController.Instance.GetCrateForFruit(m_fruitString);
        if (crate != null)
        {
            m_quantityLabel.text = $"{crate.Quantity}"; // RPB: This is in the wrong placec
        }

        UpdateView();

        var timeUntilExpiration = m_cachedCrate.ExpirationDatetime - DateTime.Now;

        if(timeUntilExpiration.TotalMinutes < 0f)
        {
            // it is expired
            m_timeToExpirationLabel.text = $"{-timeUntilExpiration.TotalMinutes:0}m";
            m_timeToExpirationLabel2.text = $"Expired";
        }
        else
        {
            // it is ticking
            m_timeToExpirationLabel.text = $"{timeUntilExpiration.TotalMinutes:0} min";
            m_timeToExpirationLabel2.text = $"To Expiration";
        }

        var timeUrgency = 0f;

        if (timeUntilExpiration.TotalMinutes < 60f)
        {
            timeUrgency = 1 - (float)timeUntilExpiration.TotalMinutes / 60f;
        }

        m_timeUrgencyForFrame = timeUrgency;

    }

    private void UpdateView()
    {
        if (PlayerController.Instance == null)
        {
            return;
        }

        m_cachedCrate = InventoryController.Instance.GetCrateForFruit(m_fruitString);

        var shouldCull = (Vector3.Distance(this.transform.position, PlayerController.Instance.transform.position) > CULL_DISTANCE);

        m_text.text = $"Urgency: {Urgency:0.00}"; //yeet

        if (Urgency > TOLERANCE_RED)
        {
            if (shouldCull)
            {
                m_template.texture = m_redCullTemplate;
            }
            else
            {
                m_template.texture = m_redTemplate;
            }

        }
        else if (Urgency > TOLERANCE_YELLOW)
        {
            if (shouldCull)
            {
                m_template.texture = m_yellowCullTemplate;
            }
            else
            {
                m_template.texture = m_yellowTemplate;
            }
        }
        else
        {
            if (shouldCull)
            {
                m_template.texture = m_greenCullTemplate;
            }
            else
            {
                m_template.texture = m_greenTemplate;
            }
        }

        m_objectsCanCull.SetActive(!shouldCull);

        if (!shouldCull)
        {
            m_displayName.text = FruitController.Instance.GetFruit(m_fruitString).DisplayName;
            m_displayImage.texture = FruitController.Instance.GetFruit(m_fruitString).DisplayImage;
        }

        m_previousShouldCull = shouldCull;
    }
}