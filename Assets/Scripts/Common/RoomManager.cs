using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    // 최대 플레이어 수를 나타내는 변수
    public byte maxPlayers = 8;

    void Start()
    {
        OnConnectedToServer();
    }

    // 서버에 연결하는 함수
    void OnConnectedToServer()
    {
        // Photon 서버에 설정을 사용하여 연결
        PhotonNetwork.ConnectUsingSettings();

        // 씬을 자동으로 동기화하도록 설정
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // 마스터 서버에 연결되었을 때 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        // 기본 OnConnectedToMaster 콜백 함수 호출
        base.OnConnectedToMaster();

        // 방 설정을 위한 RoomOptions 객체 생성
        RoomOptions roomOptions = new RoomOptions();

        // 최대 플레이어 수 설정
        roomOptions.MaxPlayers = maxPlayers;

        // 방이 리스트에 표시되도록 설정
        roomOptions.IsVisible = true;

        // 방이 열려있도록 설정
        roomOptions.IsOpen = true;

        // "Room 1"이라는 이름의 방에 참가하거나 없으면 생성
        PhotonNetwork.JoinOrCreateRoom("Room 1", roomOptions, TypedLobby.Default);
    }
}
