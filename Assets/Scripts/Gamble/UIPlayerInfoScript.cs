using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPlayerInfoScript : MonoBehaviour
{
    public TextMeshProUGUI playerRank;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerScore;

    public void SetTextUI(string rank, string name, string score)
    {
        playerRank.text = rank;
        playerName.text = name;
        playerScore.text = score;
    }
}
