using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class PlayerController : MonoBehaviour
{
    private static PlayerController s_playerController;

    [SerializeField]
    private ControllerConnectionHandler m_controller = null;

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
        if(s_playerController != null)
        {
            Debug.LogError("more than two player controllers initialized!");
            return;
        }
        else
        {
            s_playerController = this;
        }
    }
}
