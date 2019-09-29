using MagicLeap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private PCFVisualizer m_visualizer;

    [SerializeField]
    private GameObject m_anchor = null;

    [SerializeField]
    private bool m_debugPCF = false;

    private PCFStatusText[] m_PCFs = default;

    private PCFStatusText m_pcf1 = default;
    private PCFStatusText m_pcf2 = default;

    [SerializeField]
    private GameObject m_anchorHelper1 = default;

    [SerializeField]
    private GameObject m_anchorHelper2 = default;

    [SerializeField]
    private GameObject m_pcfHelper1 = default;

    [SerializeField]
    private GameObject m_pcfHelper2 = default;

    [SerializeField]
    private string m_pcfJson = null;

    [SerializeField]
    private PCFPairTable m_pcfs = default;

    private IEnumerator Start()
    {
        m_pcfs = JsonUtility.FromJson<PCFPairTable>(m_pcfJson);


        m_anchor.SetActive(false);
        yield return new WaitForSeconds(1f);
        m_visualizer.ToggleDebug();
        yield return new WaitForSeconds(1f);
        m_PCFs = FindObjectsOfType<PCFStatusText>();
        int foundIndex1 = -1;
        float shortestDistance = float.MaxValue;

        for (int i = 0; i < m_pcfs.Pairs.Length; i++)
        {
            if(!m_pcfs.Pairs[i].Enabled)
            {
                continue;
            }

            var foundPCF = GameObject.Find(m_pcfs.Pairs[i].PCFGuid); //rpb: oh god yes I know how chonky this boi is but its a hackathon, YEEET
            if (foundPCF != null)
            {
                if (foundPCF.transform.position.magnitude < shortestDistance)
                {
                    shortestDistance = foundPCF.transform.position.magnitude;
                    m_pcf1 = foundPCF.GetComponent<PCFStatusText>();
                    m_pcfHelper1.transform.position = m_pcf1.transform.position;
                    m_anchorHelper1.transform.localPosition = m_pcfs.Pairs[i].Offset;
                    //m_anchor.transform.position = foundPCF.transform.position - m_pcfs.Pairs[i].Offset;
                }
            }

        }

        var farthestDistance = float.MinValue;

        for (int i = 0; i < m_pcfs.Pairs.Length; i++)
        {
            if (!m_pcfs.Pairs[i].Enabled)
            {
                continue;
            }

            if(foundIndex1 == i)
            {
                continue;
            }

            var foundPCF = GameObject.Find(m_pcfs.Pairs[i].PCFGuid); //rpb: oh god yes I know how chonky this boi is but its a hackathon, YEEET
            if (foundPCF != null)
            {
                if (foundPCF.transform.position.magnitude > farthestDistance)
                {
                    farthestDistance = foundPCF.transform.position.magnitude;
                    m_pcf2 = foundPCF.GetComponent<PCFStatusText>();
                    m_pcfHelper2.transform.position = m_pcf2.transform.position;
                    m_anchorHelper2.transform.localPosition = m_pcfs.Pairs[i].Offset;
                }
            }
        }

        m_anchor.transform.position = m_pcfHelper1.transform.position - m_anchorHelper1.transform.position;


        Debug.LogError($"PCFs: {m_pcf1.name} and {m_pcf2.name}");

        m_anchor.SetActive(true);

        yield return new WaitForSeconds(1f);

        int counter = 0;
        while (Vector2.Distance(new Vector2(m_anchorHelper2.transform.position.x, m_anchorHelper2.transform.position.z), new Vector2(m_pcfHelper2.transform.position.x, m_pcfHelper2.transform.position.z)) > 0.1f && counter < 36001)
        {
            m_anchor.transform.RotateAround(m_pcf1.transform.position, Vector3.up, 0.01f);
            counter++;
        }
        

        //Debug.LogError($"distance {Vector3.Distance(m_anchorHelper.transform.position, m_pcf2.transform.position)}");
    }

    private void Update()
    {


        if (m_PCFs != null && m_PCFs.Length != 0 && m_debugPCF)
        {
            var pcfList = new List<PCFPair>();

            for (int i = 0; i < m_PCFs.Length; i++)
            {
                pcfList.Add(new PCFPair(m_PCFs[i].name, m_anchor.transform.InverseTransformPoint(m_PCFs[i].transform.position)));
            }

            var table = new PCFPairTable();
            table.Pairs = pcfList.ToArray();

            Debug.LogError(JsonUtility.ToJson(table));

            m_debugPCF = false;
        }
        /*
        if (Vector3.Distance(m_anchorHelper.transform.position, m_pcf2.transform.position) > 0.1f)
        {
            m_anchor.transform.RotateAround(m_pcf1.transform.position, Vector3.up, Time.deltaTime * 20f);
            Debug.LogError(m_anchorHelper.transform.position);
        }*/
    }

}

[System.Serializable]
public class PCFPair
{
    public string PCFGuid;
    public Vector3 Offset;
    public bool Enabled = true;

    public PCFPair(string guid, Vector3 offset)
    {
        PCFGuid = guid;
        Offset = offset;
        Enabled = true;
    }
}

[System.Serializable] 
public class PCFPairTable
{
    public PCFPair[] Pairs;
}