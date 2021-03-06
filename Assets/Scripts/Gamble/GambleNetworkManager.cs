using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class GambleNetworkManager : NetworkManager
{

    private void Start()
    {
        InitActorNumbers();
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                InsertActorNumber(player.ActorNumber);
            }
            canSpawn = true;
            MasterSendActorNumbers();
        }
        StartCoroutine(SpawnPlayer());
    }

    protected override void InitActorNumbers()
    {
        int size = PhotonNetwork.CurrentRoom.PlayerCount;
        actorNumbers = new int[size];
        for (int i = 0; i < actorNumbers.Length; i++)
        {
            actorNumbers[i] = -1;
        }
    }

    protected override (string, string) GetResourceName()
    {
        string localPlayer = "Local Player";
        string networkPlayer = "Network Player";
        return (localPlayer, networkPlayer);
    }

    public void SetStateToAll(State state)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PV.RPC("RPC_ReceiveState", RpcTarget.AllViaServer, state);
    }

    [PunRPC]
    private void RPC_ReceiveState(State state)
    {
        GambleManager.state = state;
    }

    public void SendActorNumberToMaster()
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        PV.RPC("RPC_ReceiveActorNumber", RpcTarget.MasterClient, actorNumber);
    }

    [PunRPC]
    private void RPC_ReceiveActorNumber(int actorNumber)
    {
        GambleManager.players.Add(actorNumber);
    }

    public void SendPlayersToOthers(PlayerInfoList _players)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        (string players, string winner, string attacker) = _players.ToJson();
        PV.RPC("RPC_ReceivePlayers", RpcTarget.Others, players, winner, attacker);
    }

    [PunRPC]
    private void RPC_ReceivePlayers(string players, string winner, string attacker)
    {
        GambleManager.players = PlayerInfoList.FromJson(players, winner, attacker);
    }

    public void SendLogToOthers(string message)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PV.RPC("RPC_ReceiveLogMessage", RpcTarget.Others, message);
    }

    [PunRPC]
    private void RPC_ReceiveLogMessage(string message)
    {
        GambleManager.LogOnBoard(message);
    }

    public void SendPotCoinsToOthers(int coins)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PV.RPC("RPC_ReceivePotCoins", RpcTarget.Others, coins);
    }

    [PunRPC]
    private void RPC_ReceivePotCoins(int coins)
    {
        GambleManager.potCoins = coins;
    }

    public void SendChoiceToMaster(Choice choice)
    {
        if (PhotonNetwork.IsMasterClient) return;
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        PV.RPC("RPC_ReceiveChoice", RpcTarget.MasterClient, actorNumber, choice);
    }

    [PunRPC]
    private void RPC_ReceiveChoice(int actorNumber, Choice choice)
    {
        GambleManager.players.SetPlayerChoice(actorNumber, choice);
    }

    public void SendChallengeAmountToMaster(int amount)
    {
        if (PhotonNetwork.IsMasterClient) return;
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        PV.RPC("RPC_ReceiveChallengeAmount", RpcTarget.MasterClient, actorNumber, amount);
    }

    [PunRPC]
    private void RPC_ReceiveChallengeAmount(int actorNumber, int amount)
    {
        GambleManager.players.DecidePlayerChallenge(actorNumber, amount);
    }

    public void SetTimerToAll(int time)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PV.RPC("RPC_ReceiveLeftTime", RpcTarget.AllViaServer, time);
    }

    [PunRPC]
    private void RPC_ReceiveLeftTime(int time)
    {
        GambleManager.leftTime = time;
    }

    public void SendAttackTargetToMaster(int targetActorNumber)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GambleManager.AttackOnMaster(targetActorNumber);
            return;
        }
        PV.RPC("RPC_ReceiveAttackTarget", RpcTarget.MasterClient, targetActorNumber);
    }

    [PunRPC]
    private void RPC_ReceiveAttackTarget(int targetActorNumber)
    {
        GambleManager.AttackOnMaster(targetActorNumber);
    }


    public void EndAct()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PV.RPC("RPC_EndAct", RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void RPC_EndAct()
    {
        GambleManager.Reward();
        GambleManager.NextAct();
        GambleManager.state = State.standBy;
    }

    [PunRPC]
    protected override void RPC_ReceiveActorNumbers(int[] _actorNumbers)
    {
        base.RPC_ReceiveActorNumbers(_actorNumbers);
    }

    internal void SendAttackResultToAll(PlayerInfoList _players, int targetActorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        (string players, string winner, string attacker) = _players.ToJson();
        PV.RPC("RPC_ReceiveAttackResult", RpcTarget.All, players, targetActorNumber);
    }

    [PunRPC]
    private void RPC_ReceiveAttackResult(string players, int targetActorNumber)
    {
        GambleManager.players = PlayerInfoList.FromJson(players);
        if (GambleManager.GetMyInfo().actorNumber == targetActorNumber)
        {
            PhotonNetwork.Destroy(spawnedPlayer);
        }
    }
}
