using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject gambleInfo;
    public GameObject endingUI;
    TextMeshProUGUI round, leftTime, potRange, myCoins;
    private Vector3 targetPosition;

    void Start()
    {
        round = gambleInfo.transform.Find("RoundText").GetComponent<TextMeshProUGUI>();
        leftTime = gambleInfo.transform.Find("LeftTimeText").GetComponent<TextMeshProUGUI>();
        potRange = gambleInfo.transform.Find("PotRangeText").GetComponent<TextMeshProUGUI>();
        myCoins = gambleInfo.transform.Find("MyCoinsText").GetComponent<TextMeshProUGUI>();
    }

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
        potRange.text = "[" + GambleManager.GetMinPotCoins() + " ~ " + GambleManager.GetMaxPotCoins() + "]";
        myCoins.text = " My : " + player.coins + "G";
    }

    public void SetEndingTextUI()
    {
        gambleInfo.SetActive(false);
        endingUI.SetActive(true);
        targetPosition = new Vector3(GambleManager.localPlayer.transform.position.x, endingUI.transform.position.y, GambleManager.localPlayer.transform.position.z);
        endingUI.transform.LookAt(targetPosition);
        endingUI.transform.Translate(new Vector3(0, 0, -1));
    }
}
