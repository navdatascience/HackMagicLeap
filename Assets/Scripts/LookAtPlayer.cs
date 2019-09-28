using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{

    void Update()
    {
        if (PlayerController.Instance != null)
        {
            transform.LookAt(PlayerController.Instance.transform);
        }
    }
}
