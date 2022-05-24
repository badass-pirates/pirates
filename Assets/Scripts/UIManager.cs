using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject roundInfo;
    TextMeshProUGUI round, act, potRange, myCoins;

    // Start is called before the first frame update
    void Start()
    {
        round = roundInfo.transform.Find("RoundText").GetComponent<TextMeshProUGUI>();
        act = roundInfo.transform.Find("ActText").GetComponent<TextMeshProUGUI>();
        potRange = roundInfo.transform.Find("PotRangeText").GetComponent<TextMeshProUGUI>();
        myCoins = roundInfo.transform.Find("MyCoinsText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GambleManager.localPlayer != null) SetTextUI();
    }

    void Initialize(GameObject player)
    {
    }
    void SetTextUI()
    {
        PlayerInfo player = GambleManager.GetMyInfo();
        if (player == null) return;

        // timeText.text = ((int)GambleManager.leftTime).ToString();
        // SetText(playerInfoText.transform.Find("ChoiceText").gameObject, player.choice.ToString());
        // SetText(playerInfoText.transform.Find("ChallengeAmountText").gameObject, player.challengeAmount.ToString());
        roundInfo.transform.LookAt(GambleManager.localPlayer.transform);
        roundInfo.transform.Rotate(0, 180, 0);
        round.text = "Round : " + GambleManager.round;
        act.text = "Act : " + GambleManager.act;
        potRange.text = GambleManager.GetMinPotCoins() + " ~ " + GambleManager.GetMaxPotCoins();
        myCoins.text = "Pot : " + GambleManager.potCoins + "My : " + player.coins + "G";
    }
}
