using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class ChallengeAmount : MonoBehaviour
{
    public XRController controller;
    public GameObject chellengeCanvas;
    public Image amountLeft;
    public Image amountRight;
    public Text amountText;

    private int amount;

    public int Amount { get => amount; set => amount = value; }

    void Start()
    {
        controller = GameObject.Find("LeftHand Controller").GetComponent<XRController>();
    }

    void Update()
    {
        if (chellengeCanvas.activeSelf == true)
        {
            DecideAmount();
            amountText.text = Amount.ToString();
        }
    }

    private void DecideAmount()
    {
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 position))
        {
            if (position.x > 0) // thumb stick Right movement
            {
                Amount++;
            }
            else if (position.x < 0) // thumb stick Left movement
            {
                Amount--;
            }
        }
    }
}
