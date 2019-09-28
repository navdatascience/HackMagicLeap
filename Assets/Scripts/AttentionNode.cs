using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttentionNode : MonoBehaviour
{
    private static List<AttentionNode> s_attentionNodes = new List<AttentionNode>();

    private const float TOLERANCE_RED = 0.8f;
    private const float TOLERANCE_YELLOW = 0.5f;
    private const float CULL_DISTANCE = 20f;

    [SerializeField]
    private float m_urgency = 1;

    [SerializeField]
    private Text m_text;

    [SerializeField]
    private RawImage m_template;

    [SerializeField]
    private Texture m_redTemplate;


    [SerializeField]
    private Texture m_yellowTemplate;


    [SerializeField]
    private Texture m_greenTemplate;


    [SerializeField]
    private Texture m_yellowCullTemplate;


    [SerializeField]
    private Texture m_greenCullTemplate;


    [SerializeField] //lol
    private Texture m_redCullTemplate;


    [SerializeField]
    private GameObject m_objectsCanCull;

    [SerializeField]
    private Text m_displayName;

    [SerializeField]
    private RawImage m_displayImage;

    [SerializeField]
    private string m_fruitString = "FILL ME IN";

    private float m_previousUrgency = float.MinValue;
    private bool m_previousShouldCull = false;

    public float Urgency
    {
        get { return m_urgency; }
    }

    public static List<AttentionNode> AttentionNodes
    {
        get { return s_attentionNodes; }
    }

    private void Start()
    {
        s_attentionNodes.Add(this);
        Debug.Log($"Nodes: {s_attentionNodes.Count}");
        UpdateView();
    }

    private void Update()
    {
        var shouldCull = (Vector3.Distance(this.transform.position, PlayerController.Instance.transform.position) > CULL_DISTANCE);


        if (m_previousShouldCull !=  shouldCull || m_urgency != m_previousUrgency)
        {
            UpdateView();
        }
    }

    private void UpdateView()
    {
        var shouldCull = (Vector3.Distance(this.transform.position, PlayerController.Instance.transform.position) > CULL_DISTANCE);

        m_text.text = $"Urgency: {m_urgency:0.00}";

        if (m_urgency > TOLERANCE_RED)
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
        else if (m_urgency > TOLERANCE_YELLOW)
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

        if(!shouldCull)
        {
            m_displayName.text = FruitController.Instance.GetFruit(m_fruitString).DisplayName;
            m_displayImage.texture = FruitController.Instance.GetFruit(m_fruitString).DisplayImage;
        }

        m_previousUrgency = m_urgency;
        m_previousShouldCull = shouldCull;
    }
}