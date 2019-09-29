using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericButton : MonoBehaviour
{

    public UnityEvent OnButtonDown;
    public UnityEvent OnButtonUp;

    [SerializeField]
    private SphereCollider m_collider = null;

    [SerializeField]
    private float m_worldSpaceRadius = 0.08f;

    private bool m_buttonIsDown = false;

    public bool IsIntersecting = default;
    public bool IsInteractingAndButtonDown = default;

    public void DestroySelf()
    {
        GameObject.Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.Instance == null || PlayerController.Instance.Controller == null || PlayerController.Instance.Controller.ConnectedController == null)
        {
            //Debug.LogError("null stuff");
            return;
        }

        var controllerPosition = PlayerController.Instance.Controller.ConnectedController.Position;
        var distanceToController = Vector3.Distance(controllerPosition, this.transform.position);

        //Debug.LogError(PlayerController.Instance.Controller.ConnectedController.TriggerValue);

        var buttonIsDown = false;

        IsIntersecting = distanceToController < m_worldSpaceRadius;
        IsInteractingAndButtonDown = false;

        if (PlayerController.Instance.Controller.ConnectedController.TriggerValue > 0.5f && distanceToController < m_worldSpaceRadius)
        {
            buttonIsDown = true;
            IsInteractingAndButtonDown = true;
        }

        if (buttonIsDown != m_buttonIsDown)
        {
            if (buttonIsDown)
            {
                OnButtonDown.Invoke();
            }
            else
            {
                OnButtonUp.Invoke();
            }

            m_buttonIsDown = buttonIsDown;
        }
    }
}
