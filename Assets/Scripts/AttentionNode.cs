using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttentionNode : MonoBehaviour
{
    private static List<AttentionNode> s_attentionNodes = new List<AttentionNode>();

    [SerializeField]
    private float m_urgency = 1;

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
    }
}
