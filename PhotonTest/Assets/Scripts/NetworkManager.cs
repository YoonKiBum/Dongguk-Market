using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
  public Text StatusText;//접속상태
  public InputField NickNameInput;//닉네임
  public GameObject Cube;//플레이어 생성 위치
  public string gameVersion = "1.0";//게임 버전


  void Awake()
  {
      //마스터 client와 일반 client의 레벨을 동기화 할지 결정한다. true일 시, 마스터 client에서 레벨 변경 시 모든 client들이 자동으로 동일한 레벨을 로드한다.
      PhotonNetwork.AutomaticallySyncScene = true;
  }

  void Start()
  {
      //포톤 게임 버전을 gameVersion으로 설정한다.
      PhotonNetwork.GameVersion = this.gameVersion;
      //온라인 연결 수행. 연결이 성공하면 콜백 메소드인 OnConnectedToMaster가 실행
      PhotonNetwork.ConnectUsingSettings();
  }

  void Connect() => PhotonNetwork.ConnectUsingSettings();

  public override void OnConnectedToMaster()
  {
      //생성된 방에 랜덤하게 접속한다. 
      //접속 실패 시, 콜백 메소드인 OnJoinRandomFailed가 실행되고 PhotonNetwork.CreateRoom을 통해 새로운 방을 생성한다. 
      //접속 성공 시, OnJoinedRoom가 실행되고, PhotonNetwork.Instantiate을 통해 플레이어를 생성한다.
      PhotonNetwork.JoinRandomRoom();
  }

  public override void OnJoinedRoom()
  {
      PhotonNetwork.Instantiate("Player3", Cube.transform.position, Quaternion.identity);
  }

  public override void OnJoinRandomFailed(short returnCode, string message)
  {
      this.CreateRoom();
  }

  void CreateRoom()
  {
      PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
  }
}