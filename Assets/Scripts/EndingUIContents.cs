using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingUIContents : MonoBehaviour
{
    private string[] playerName;
    private int[] playerScore;
    public GameObject playerInfoItem;
    public GameObject otherInfoItem;

    void Start()
    {
        init();
    }

    private void init()
    {
        foreach (PlayerInfo player in GambleManager.players.GetList())
        {
            if (GambleManager.GetMyInfo().name == player.name)
            {
                GameObject q = Instantiate(playerInfoItem);
                q.GetComponent<UIPlayerInfoScript>().SetTextUI(player.actorNumber.ToString(), player.name, player.coins.ToString());
                q.transform.SetParent(transform, false);
            }
            else
            {
                GameObject q = Instantiate(otherInfoItem);
                q.GetComponent<UIPlayerInfoScript>().SetTextUI(player.actorNumber.ToString(), player.name, player.coins.ToString());
                q.transform.SetParent(transform, false);
            }
        }
    }
}
