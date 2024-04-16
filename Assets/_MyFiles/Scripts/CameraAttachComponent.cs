using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAttachComponent : MonoBehaviour
{
    private Transform targetAttachPoint;

    public void SetupAttachPoint(Transform attachPoint)
    {
        targetAttachPoint = attachPoint;
    }

    private void Update()
    {
        if (targetAttachPoint)
        {
            transform.position = Camera.main.WorldToScreenPoint(targetAttachPoint.position);
        }
    }

}
