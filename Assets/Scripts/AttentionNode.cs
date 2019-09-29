using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AttentionNode : MonoBehaviour
{
    private static List<AttentionNode> s_attentionNodes = new List<AttentionNode>();

    private const float TOLERANCE_RED = 0.5f;
    private const float TOLERANCE_YELLOW = 0.0f;
    private const float CULL_DISTANCE = 2f;

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

    [SerializeField]
    private int m_formDataCrateId = default;

    [SerializeField]
    private string m_formDataItemName = default;

    [SerializeField]
    private int m_formDataQuantity = default;

    [SerializeField]
    private int m_formDataExpirationHours = default;

    [SerializeField]
    private int m_formDataExpirationMinutes = default;

    [SerializeField]
    private AudioSource m_expiredAlarm;

    [SerializeField]
    private AudioSource m_healthyAlarm;

    [SerializeField]
    private AudioSource m_warningAlarm;

    [SerializeField]
    private AudioSource m_criticalAlarm;

    [SerializeField]
    private Text m_restockTextButton;

    [SerializeField]
    private GameObject m_criticalParticles;

    [SerializeField]
    private GameObject m_extremeParticles;

    [SerializeField]
    private GenericButton m_associatedButton;

    [SerializeField]
    private MeshRenderer m_renderer;

    private float m_timeUrgencyForFrame = 0f;

    private Coroutine m_coroutine = null;

    private NodeStatus m_currentNodeStatus = NodeStatus.Healthy;

    public static DateTime TimeUntilCanRestockAgain = default;

    private static bool s_isRestocking = false;

    public float Urgency
    {
        get
        {
            if (m_cachedCrate != null)
            {
                var quantityUrgency = Mathf.Clamp01(((float)HEALTHY_STOCK_QUANTITY - (float)m_cachedCrate.quantity) / (float)HEALTHY_STOCK_QUANTITY);



                //Debug.LogError($"time till expiration: {timeUntilExpiration.TotalMinutes}");

                if (m_timeUrgencyForFrame > quantityUrgency)
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
        m_warningAlarm.volume = 1f;
        m_warningAlarm.Stop();
        m_warningAlarm.loop = false;

        m_criticalAlarm.volume = 1f;
        m_criticalAlarm.Stop();
        m_criticalAlarm.loop = false;

        m_restockTextButton.text = $"Pull Trigger\n to restock to\n{m_formDataQuantity} items";

        m_cachedCrate = new Crate();
        m_cachedCrate.crate_id = (int)UnityEngine.Random.Range(0f, 1000f);
        var simulatedExpirationTime = DateTime.UtcNow.AddMinutes(UnityEngine.Random.Range(2f, 6f));
        m_cachedCrate.expiration_datetime = simulatedExpirationTime.ToString();
        m_cachedCrate.item_name = m_fruitString;
        m_cachedCrate.last_stock_datetime = simulatedExpirationTime.AddMinutes(-m_formDataExpirationMinutes).ToString();
        m_cachedCrate.quantity = (int)UnityEngine.Random.Range(0f, m_formDataQuantity);

        UpdateView();
    }

    private void Update()
    {
        if (UnityEngine.Random.Range(0f, 1f) < 0.002f && m_cachedCrate.quantity > 0)
        {
            m_cachedCrate.quantity -= 1;
        }
        var shouldCull = (Vector3.Distance(this.transform.position, PlayerController.Instance.transform.position) > CULL_DISTANCE);

        UpdateView();

        if (m_cachedCrate != null)
        {
            m_quantityLabel.text = $"{m_cachedCrate.quantity}"; // RPB: This is in the wrong placec

            var timeUntilExpiration = m_cachedCrate.ExpirationDatetime - DateTime.UtcNow;

            bool shouldShowExtremeParticles = false;

            if (timeUntilExpiration.TotalMinutes < 0f)
            {
                // it is expired
                var timePastExpiration = DateTime.UtcNow - m_cachedCrate.ExpirationDatetime;
                m_timeToExpirationLabel.text = $"{Mathf.FloorToInt((float)timePastExpiration.TotalHours):00}:{timePastExpiration.Minutes:00}:{timePastExpiration.Seconds:00}";
                m_timeToExpirationLabel2.text = $"Expired";
                shouldShowExtremeParticles = true;
                m_expiredAlarm.volume = 1f;
            }
            else
            {
                // it is ticking
                m_timeToExpirationLabel.text = $"{Mathf.FloorToInt((float)timeUntilExpiration.TotalHours):00}:{timeUntilExpiration.Minutes:00}:{timeUntilExpiration.Seconds:00}";
                m_timeToExpirationLabel2.text = $"Fresh";
                m_expiredAlarm.volume = 0f;
            }

            if (m_cachedCrate.quantity == 0)
            {
                m_expiredAlarm.volume = 1f;
                shouldShowExtremeParticles = true;
            }

            m_extremeParticles.SetActive(shouldShowExtremeParticles);

            var timeUrgency = 0f;

            if (timeUntilExpiration.TotalMinutes < 2f)
            {
                timeUrgency = 1 - (float)timeUntilExpiration.TotalMinutes / 2f;
            }

            m_timeUrgencyForFrame = timeUrgency;
        }

    }

    private void UpdateView()
    {
        if (PlayerController.Instance == null)
        {
            return;
        }

        //m_cachedCrate = InventoryController.Instance.GetCrateForFruit(m_fruitString);

        var shouldCull = (Vector3.Distance(this.transform.position, PlayerController.Instance.transform.position) > CULL_DISTANCE);

        m_text.text = $"Urgency: {Urgency:0.00}"; //yeet

        //var angleBetween = Vector2.SignedAngle(new Vector2(PlayerController.Instance.transform.position.x, PlayerController.Instance.transform.position.z), new Vector2(transform.position.x, transform.position.z));

        var lookDirectionFromPlayer = PlayerController.Instance.transform.forward;
        var directionFromPlayerToNode = transform.position - PlayerController.Instance.transform.position;

        var angleBetween = Vector3.SignedAngle(lookDirectionFromPlayer, directionFromPlayerToNode, Vector3.up);

        bool isLeft = false;

        if (angleBetween < 0)
        {
            isLeft = true;
        }

        if (Urgency > TOLERANCE_RED)
        {
            shouldCull = false;
            m_template.texture = m_redTemplate;

            if (m_currentNodeStatus != NodeStatus.Critical)
            {
                m_currentNodeStatus = NodeStatus.Critical;
                m_criticalAlarm.Play();
                m_criticalParticles.SetActive(true);
                if (isLeft)
                {
                    PlayerController.Instance.ShowLeftCritical();
                }
                else
                {
                    PlayerController.Instance.ShowRightCritical();
                }
            }

            m_renderer.material.color = Color.red;
        }
        else if (Urgency > TOLERANCE_YELLOW)
        {
            m_criticalParticles.SetActive(false);
            if (shouldCull)
            {
                m_template.texture = m_yellowCullTemplate;
            }
            else
            {
                m_template.texture = m_yellowTemplate;
            }

            if (m_currentNodeStatus != NodeStatus.Warning)
            {
                m_currentNodeStatus = NodeStatus.Warning;
                m_warningAlarm.Play();

                if (isLeft)
                {
                    PlayerController.Instance.ShowLeftWarning();
                }
                else
                {
                    PlayerController.Instance.ShowRightWarning();
                }
            }

            m_renderer.material.color = Color.yellow;
        }
        else
        {
            m_criticalParticles.SetActive(false);
            if (shouldCull)
            {
                m_template.texture = m_greenCullTemplate;
            }
            else
            {
                m_template.texture = m_greenTemplate;
            }

            if (m_currentNodeStatus != NodeStatus.Healthy)
            {
                m_currentNodeStatus = NodeStatus.Healthy;
                m_healthyAlarm.Play();
            }

            m_renderer.material.color = Color.green;
        }

        m_objectsCanCull.SetActive(!shouldCull);

        if (!shouldCull)
        {
            m_displayName.text = FruitController.Instance.GetFruit(m_fruitString).DisplayName;
            m_displayImage.texture = FruitController.Instance.GetFruit(m_fruitString).DisplayImage;
        }

        m_previousShouldCull = shouldCull;

        if (m_associatedButton.IsIntersecting)
        {
            // make bright
            if (m_associatedButton.IsInteractingAndButtonDown)
            {
                //make brighter
            }
            else
            {
                m_renderer.material.color = new Color(m_renderer.material.color.r * 0.8f, m_renderer.material.color.g * 0.8f, m_renderer.material.color.b * 0.8f);
                //make not as bright
            }
        }
        else
        {
            m_renderer.material.color = new Color(m_renderer.material.color.r * 0.4f, m_renderer.material.color.g * 0.4f, m_renderer.material.color.b * 0.4f);
            // make dull
        }
    }

    public void SendRestockRequest()
    {
        if (s_isRestocking == false)
        {
            m_cachedCrate.quantity = m_formDataQuantity;
            m_cachedCrate.last_stock_datetime = DateTime.UtcNow.ToString();
            m_cachedCrate.expiration_datetime = DateTime.UtcNow.AddMinutes(m_formDataExpirationMinutes).ToString();

            StartCoroutine(RestockTimer());
        }
        else
        {
            PlayerController.Instance.ShowMustWaitVisuals();
        }
    }

    IEnumerator RestockTimer()
    {
        s_isRestocking = true;
        TimeUntilCanRestockAgain = DateTime.UtcNow.AddSeconds(6f);
        yield return new WaitForSeconds(0.01f);
        s_isRestocking = false;
    }

    IEnumerator UpdateInventoryToWeb()
    {
        WWWForm newData = new WWWForm();
        newData.AddField("crate_id", m_formDataCrateId);
        newData.AddField("item_name", m_formDataItemName);
        newData.AddField("quantity", m_formDataQuantity);
        newData.AddField("expiration_hours", m_formDataExpirationHours);
        newData.AddField("expiration_minutes", m_formDataExpirationMinutes);


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

        InventoryController.Instance.RefreshAllData();
        m_coroutine = null;
    }

}

public enum NodeStatus
{
    Healthy,
    Warning,
    Critical,
    Expired
}