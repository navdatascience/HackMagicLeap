using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelloWorld : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Rico says hello!");
    }

    void Update()
    {
        transform.RotateAround(Vector3.up, Time.deltaTime);
    }
}
