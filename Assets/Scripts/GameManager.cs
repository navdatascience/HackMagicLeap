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

    [SerializeField]
    private List<PCFPairs> m_favoritePCFs = null;

    private PCFStatusText[] m_PCFs = default;

    private PCFStatusText m_pcf1 = default;
    private PCFStatusText m_pcf2 = default;

    [SerializeField]
    private GameObject m_anchorHelper = default;

    private IEnumerator Start()
    {
        m_anchor.SetActive(false);
        yield return new WaitForSeconds(1f);
        m_visualizer.ToggleDebug();
        yield return new WaitForSeconds(1f);
        m_PCFs = FindObjectsOfType<PCFStatusText>();
        int foundIndex1 = -1;

        for (int i = 0; i < m_favoritePCFs.Count; i++)
        {
            if(!m_favoritePCFs[i].Enabled)
            {
                continue;
            }

            var foundPCF = GameObject.Find(m_favoritePCFs[i].PCFGuid); //rpb: oh god yes I know how chonky this boi is but its a hackathon, YEEET
            if (foundPCF != null)
            {
                Debug.LogError($"basing on PCF guid {m_favoritePCFs[i].PCFGuid}");
                m_anchor.transform.position = foundPCF.transform.position - m_favoritePCFs[i].Offset;
                m_pcf1 = foundPCF.GetComponent<PCFStatusText>();
                foundIndex1 = i;
                break;
            }

        }


        for (int i = 0; i < m_favoritePCFs.Count; i++)
        {
            if (!m_favoritePCFs[i].Enabled)
            {
                continue;
            }

            if(foundIndex1 == i)
            {
                continue;
            }

            var foundPCF = GameObject.Find(m_favoritePCFs[i].PCFGuid); //rpb: oh god yes I know how chonky this boi is but its a hackathon, YEEET
            if (foundPCF != null)
            {
                Debug.LogError($"2n basing on PCF guid {m_favoritePCFs[i].PCFGuid} - {m_favoritePCFs[i].Offset}");
                //m_anchor.transform.position = foundPCF.transform.position - m_favoritePCFs[i].Offset;
                //m_pcf1 = foundPCF.GetComponent<PCFStatusText>();
                m_pcf2 = foundPCF.GetComponent<PCFStatusText>();
                m_anchorHelper.transform.position = m_pcf2.transform.position;
                break;
            }
        }

        m_anchor.SetActive(true);

        yield return new WaitForSeconds(1f);

        while (Vector3.Distance(m_anchorHelper.transform.position, m_pcf2.transform.position) > 0.01f)
        {
            m_anchor.transform.RotateAround(m_pcf1.transform.position, Vector3.up, 0.01f);
        }
    }

    private void Update()
    {
        if (m_PCFs != null && m_PCFs.Length != 0 && m_debugPCF)
        {
            string PCFString = string.Empty;

            for (int i = 0; i < m_PCFs.Length; i++)
            {
                PCFString += $"PCF: {m_PCFs[i].name}, Offset: {m_PCFs[i].transform.position - m_anchor.transform.position}\n";
            }

            Debug.LogError(PCFString);

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
public class PCFPairs
{
    public string PCFGuid;
    public Vector3 Offset;
    public bool Enabled = true;
}