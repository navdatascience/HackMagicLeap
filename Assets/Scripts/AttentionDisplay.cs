﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttentionDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMesh m_text;

    [SerializeField]
    private MeshRenderer m_renderer;

    private Material m_materialCopy;

    private void Start()
    {
        m_materialCopy = new Material(m_renderer.material);
        m_renderer.material = m_materialCopy;
    }
    // Update is called once per frame
    void Update()
    {
        float MAX_DISTANCE = 5f;
        float BLUE = 0.8f;
        float RED = 0f;
        float OVERFLOW_MULTIPLIER = 1.2f;

        var nodes = AttentionNode.AttentionNodes;
        float highestStrength = float.MinValue;
        for(int i = 0; i < nodes.Count; i++)
        {
            var nodePositionXYNormalized = new Vector2(nodes[i].transform.position.x, nodes[i].transform.position.z);
            var selfPositionXYNormalized = new Vector2(transform.position.x,  transform.position.z);

            var distance = Vector2.Distance(nodePositionXYNormalized, selfPositionXYNormalized);
            var urgency = nodes[i].Urgency;
            var strength = Mathf.Clamp01(urgency * Mathf.Clamp((MAX_DISTANCE - distance), 0, MAX_DISTANCE)/MAX_DISTANCE);
            Debug.Log($"Distance to node: {distance} ");
            if(strength > highestStrength)
            {
                highestStrength = strength * OVERFLOW_MULTIPLIER;
            }
        }

        if(highestStrength > 0.05)
        {
            m_materialCopy.color = Color.HSVToRGB(Mathf.Lerp(BLUE, RED, highestStrength), 1, 1);
        }
        else
        {
            m_materialCopy.color = Color.HSVToRGB(0f, 0f, 0.5f);
        }

        m_text.text = $"{highestStrength:0.00}";
    }
}
