using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class ChallengeAmount : MonoBehaviour
{
    public GameObject challengeCanvas;
    public Image amountLeft;
    public Image amountRight;
    public Text amountText;
    public float amount { get; private set; }

    private XRController controller;

    float velocity = 1;

    void Start()
    {
        controller = GameObject.Find("RightHand Controller").GetComponent<XRController>();
        amount = GambleManager.GetMinPotCoins();
    }

    void Update()
    {
        if (challengeCanvas.activeSelf == true)
        {
            DecideAmount();
            amountText.text = ((int)amount).ToString();
        }
    }

    private void DecideAmount()
    {
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 position))
        {
            velocity *= 1.05f;

            if (position.x > 0 && amount < GambleManager.GetMaxPotCoins()) // thumb stick Right movement
            {
                amount += Time.deltaTime * velocity;
            }
            else if (position.x < 0 && (int)amount > GambleManager.GetMinPotCoins()) // thumb stick Left movement
            {
                amount -= Time.deltaTime * velocity;
            }
            else
            {
                velocity = 1;
                Mathf.Ceil(amount);
            }
        }
    }
}
