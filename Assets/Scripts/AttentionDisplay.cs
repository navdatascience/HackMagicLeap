using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttentionDisplay : MonoBehaviour
{


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
        float totalStrength = 0;
        for (int i = 0; i < nodes.Count; i++)
        {
            var nodePositionXYNormalized = new Vector2(nodes[i].transform.position.x, nodes[i].transform.position.z);
            var selfPositionXYNormalized = new Vector2(transform.position.x, transform.position.z);

            var distance = Vector2.Distance(nodePositionXYNormalized, selfPositionXYNormalized);
            var urgency = nodes[i].Urgency;
            var strength = Mathf.Clamp01(urgency * Mathf.Clamp((MAX_DISTANCE - distance), 0, MAX_DISTANCE) / MAX_DISTANCE);

            if (strength > 0)
            {
                totalStrength += strength * OVERFLOW_MULTIPLIER;
            }
        }

        if (totalStrength > 0.05)
        {
            m_materialCopy.color = Color.HSVToRGB(Mathf.Lerp(BLUE, RED, Mathf.Clamp01(totalStrength)), 1, 1);
        }
        else
        {
            m_materialCopy.color = Color.HSVToRGB(0f, 0f, 0.5f);
        }


    }
}
