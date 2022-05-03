using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionController : MonoBehaviour
{
    public XRRayInteractor leftInteractorRay;
    public XRRayInteractor rightInteractorRay;

    public bool EnableLeftRay { get; set; } = true;
    public bool EnableRightRay { get; set; } = true;

    void Update()
    {
        if (leftInteractorRay)
        {
            leftInteractorRay.gameObject.SetActive(EnableLeftRay);
        }
        if (rightInteractorRay)
        {
            rightInteractorRay.gameObject.SetActive(EnableRightRay);
        }
    }
}