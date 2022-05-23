using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class ChallengeAmount : MonoBehaviour
{
    public XRController controller;
    public GameObject challengeCanvas;
    public Image amountLeft;
    public Image amountRight;
    public Text amountText;


    public int amount { get; private set; }

    void Start()
    {
        controller = GameObject.Find("LeftHand Controller").GetComponent<XRController>();
    }

    void Update()
    {
        if (challengeCanvas.activeSelf == true)
        {
            DecideAmount();
            amountText.text = amount.ToString();
        }
    }

    private void DecideAmount()
    {
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 position))
        {
            if (position.x > 0) // thumb stick Right movement
            {
                amount++;
            }
            else if (position.x < 0) // thumb stick Left movement
            {
                amount--;
            }
        }
    }
}
