using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject gambleInfo;
    TextMeshProUGUI round, leftTime, potRange, myCoins;

    // Start is called before the first frame update
    void Start()
    {
        round = gambleInfo.transform.Find("RoundText").GetComponent<TextMeshProUGUI>();
        leftTime = gambleInfo.transform.Find("LeftTimeText").GetComponent<TextMeshProUGUI>();
        potRange = gambleInfo.transform.Find("PotRangeText").GetComponent<TextMeshProUGUI>();
        myCoins = gambleInfo.transform.Find("MyCoinsText").GetComponent<TextMeshProUGUI>();
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
        gambleInfo.transform.LookAt(GambleManager.localPlayer.transform);
        gambleInfo.transform.Rotate(0, 180, 0);
        round.text = GambleManager.round + "-" + GambleManager.act;
        leftTime.text = ((int)GambleManager.leftTime).ToString();
        potRange.text = "["+GambleManager.GetMinPotCoins() + " ~ " + GambleManager.GetMaxPotCoins()+"]";
        myCoins.text = " My : " + player.coins + "G";
    }
}
