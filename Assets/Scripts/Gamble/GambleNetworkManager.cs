using Photon.Pun;
using Photon.Realtime;

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

    public void SetState(State state)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PV.RPC("RPC_SetState", RpcTarget.AllViaServer, state);
    }

    [PunRPC]
    private void RPC_SetState(State state)
    {
        GambleManager.state = state;
    }

    public void SendActorNumberToMaster()
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        PV.RPC("RPC_ReciveMasterActorNumber", RpcTarget.MasterClient, actorNumber);
    }

    [PunRPC]
    private void RPC_ReciveMasterActorNumber(int actorNumber)
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

    public void Choice(Choice choice)
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

    public void SetTimer(int time)
    {
        if (PhotonNetwork.IsMasterClient) return;
        PV.RPC("RPC_ReceiveLeftTime", RpcTarget.Others, time);
    }

    [PunRPC]
    private void RPC_ReceiveLeftTime(int time)
    {
        GambleManager.leftTime = time;
    }

    public void EndAct()
    {
        if (PhotonNetwork.IsMasterClient) return;
        PV.RPC("RPC_EndAct", RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void RPC_EndAct()
    {
        GambleManager.Reward();
        GambleManager.NextAct();
        GambleManager.state = State.standBy;
    }
}
