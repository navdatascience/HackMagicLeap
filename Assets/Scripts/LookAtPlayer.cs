using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{

    void Update()
    {
        if (PlayerController.PlayerConstoller != null)
        {
            transform.LookAt(PlayerController.PlayerConstoller.transform);
        }
    }
}
