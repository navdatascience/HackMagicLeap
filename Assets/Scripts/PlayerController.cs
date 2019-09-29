using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class PlayerController : MonoBehaviour
{
    private static PlayerController s_playerController;

    [SerializeField]
    private ControllerConnectionHandler m_controller = null;

    [SerializeField]
    private GameObject m_mustWaitVisuals;

    [SerializeField]
    private GameObject m_criticalLeft;

    [SerializeField]
    private GameObject m_criticalRight;

    [SerializeField]
    private GameObject m_warningLeft;

    [SerializeField]
    private GameObject m_warningRight;

    [SerializeField]
    private TextMesh m_totalsText;

    [SerializeField]
    private TextMesh m_timeLeftText;

    [SerializeField]
    private GameObject m_managerWarning;

    private Coroutine m_showWaitVisualsCoroutine = null;

    private int m_warningsCount = 0;
    private int m_criticalCount = 0;

    public static PlayerController Instance
    {
        get { return s_playerController; }
    }

    public ControllerConnectionHandler Controller
    {
        get { return m_controller; }
    }

    private void Start()
    {
        if (s_playerController != null)
        {
            Debug.LogError("more than two player controllers initialized!");
            return;
        }
        else
        {
            s_playerController = this;
        }

        m_mustWaitVisuals.SetActive(false);
    }

    public void ShowMustWaitVisuals()
    {
        ShowHudItem(m_mustWaitVisuals);
    }

    public void ShowLeftWarning()
    {
        ShowHudItem(m_warningLeft);
        m_warningsCount++;
    }

    public void ShowRightWarning()
    {
        ShowHudItem(m_warningRight);
        m_warningsCount++;
    }

    public void ShowLeftCritical()
    {
        ShowHudItem(m_criticalLeft);
        m_criticalCount++;
    }

    public void ShowRightCritical()
    {
        ShowHudItem(m_criticalRight);
        m_criticalCount++;
    }


    public void ShowHudItem(GameObject m_objectToShow)
    {
        m_showWaitVisualsCoroutine = StartCoroutine(ShowMustWaitVisualsCoroutine(m_objectToShow));
    }

    private void Update()
    {
        m_totalsText.text = $"Total Warnings : {m_warningsCount}\nTotal Criticals: {m_criticalCount}";

        m_managerWarning.SetActive(m_criticalCount > 10);

        var timeUntilRestock = (AttentionNode.TimeUntilCanRestockAgain - DateTime.UtcNow).TotalSeconds;

        m_timeLeftText.text = $"You must wait {timeUntilRestock:0} sec\nbefore restocking\nagain";
    }

    private IEnumerator ShowMustWaitVisualsCoroutine(GameObject m_objectToShow)
    {
        m_objectToShow.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        m_objectToShow.SetActive(false);
        m_showWaitVisualsCoroutine = null;
    }

}
