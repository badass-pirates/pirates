using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public GameObject playerInfoText, gambleInfoText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GambleManager.localPlayer != null) SetTextUI();
    }

    public void SetText(GameObject obj, string text)
    {
        obj.GetComponent<TextMeshProUGUI>().text = text;
    }
    void SetTextUI()
    {
        PlayerInfo player = GambleManager.localPlayerInfo;
        timeText.text = ((int)GambleManager.leftTime).ToString();
        SetText(playerInfoText.transform.Find("ChoiceText").gameObject, player.choice.ToString());
        SetText(playerInfoText.transform.Find("ChallengeAmountText").gameObject, player.challengeAmount.ToString());
        SetText(playerInfoText.transform.Find("CoinsText").gameObject, player.coins.ToString() + "G");


        SetText(gambleInfoText.transform.Find("RoundText").gameObject, "Round " + GambleManager.round.ToString());
        SetText(gambleInfoText.transform.Find("ActText").gameObject, "Act " + GambleManager.act.ToString());
        SetText(gambleInfoText.transform.Find("StateText").gameObject, GambleManager.state.ToString());
        SetText(gambleInfoText.transform.Find("PotCoinsText").gameObject, "Pot:" + GambleManager.potCoins.ToString());
    }
}
