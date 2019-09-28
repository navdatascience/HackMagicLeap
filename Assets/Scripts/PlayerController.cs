using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController s_playerController;

    public static PlayerController PlayerConstoller
    {
        get { return s_playerController; }
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
