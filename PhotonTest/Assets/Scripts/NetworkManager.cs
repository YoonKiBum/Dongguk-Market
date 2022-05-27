
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject LoginObject;
    public InputField EmailInput, PasswordInput, UsernameInput;

    public Text StatusText;//접속상태
                           // public InputField NickNameInput;//닉네임
    public GameObject Cube;//플레이어 생성 위치
    public string gameVersion = "1.0";//게임 버전


    public static int MAX_PLAYER = 10;
    public static int MAX_Items = 10;
    GameObject ItemTemplate;
    [SerializeField] Transform ShopScrollView;
    public List<GameObject> Shop_Items = new List<GameObject>();
    public GameObject Sample_Item; // 상점에 인스턴스 생성하기 위해 만들어 놓은 sample
    public GameObject shop;
    public GameObject register;
    public InputField NameInput, PriceInput, InfoInput;

    Hashtable Player_Items = new Hashtable();

    public PlayerLeaderboardEntry MyPlayFabInfo;
    public List<PlayerLeaderboardEntry> PlayFabUserList = new List<PlayerLeaderboardEntry>();



    bool Flag = false;

    void Awake()
    {
        //마스터 client와 일반 client의 레벨을 동기화 할지 결정한다. true일 시, 마스터 client에서 레벨 변경 시 모든 client들이 자동으로 동일한 레벨을 로드한다.
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        //포톤 게임 버전을 gameVersion으로 설정한다.
        PhotonNetwork.GameVersion = this.gameVersion;
        Sample_Item.SetActive(false);
        shop.SetActive(false);
        register.SetActive(false);
    }

    public void LoginBtn()
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }


    public void RegisterBtn()
    {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text, DisplayName = UsernameInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => { print("회원가입 성공"); SetStat(); SetData("0"); }, (error) => print("회원가입 실패"));
    }

    void SetStat()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "IDInfo", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => print("값 저장실패"));
    }

    void SetData(string initNo)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>() { { "ItemNo", initNo } },
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(request, (result) => print("데이터 저장 성공"), (error) => print("데이터 저장 실패"));
    }

    void GetLeaderboard(string myID)
    {
        PlayFabUserList.Clear();

        for (int i = 0; i < MAX_PLAYER; i++)
        {
            var request = new GetLeaderboardRequest
            {
                StartPosition = i * 100,
                StatisticName = "IDInfo",
                MaxResultsCount = 100,
                ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true }
            };
            PlayFabClientAPI.GetLeaderboard(request, (result) =>
            {
                if (result.Leaderboard.Count == 0) return;
                for (int j = 0; j < result.Leaderboard.Count; j++)
                {
                    PlayFabUserList.Add(result.Leaderboard[j]);
                    if (result.Leaderboard[j].PlayFabId == myID) MyPlayFabInfo = result.Leaderboard[j];
                }

                getUserItemNo();
            },
            (error) => { });
        }
    }

    public void getUserItemNo()
    {
        for (int i = 0; i < PlayFabUserList.Count; i++)
        {
            var request = new GetUserDataRequest() { PlayFabId = PlayFabUserList[i].PlayFabId };
            PlayFabClientAPI.GetUserData(request,
                (result) => {
                    if (result.Data == null || !result.Data.ContainsKey("ItemNo")) Debug.Log("No Data");
                    else
                    {
                        string t = result.Data["ItemNo"].Value;

                        if (!Player_Items.ContainsKey(PlayFabUserList[i].PlayFabId))
                        {
                            Player_Items.Add(PlayFabUserList[i].PlayFabId, t);
                        }
                        else
                            Player_Items[PlayFabUserList[i].PlayFabId] = t;
                    }
                }, (error) => {
                    Debug.Log("Got error retrieving user data:");
                });
        }
    }



    public void UploadBtn()
    {
        string name = NameInput.text;
        string price = PriceInput.text;
        string Info = InfoInput.text;

        string data = name + "`" + price + "`" + Info;
        string No = "0";

        if (Player_Items.ContainsKey(MyPlayFabInfo.PlayFabId))
        {
            No = Player_Items[MyPlayFabInfo.PlayFabId].ToString();
        }

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>() { { No, data } },
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(request, (result) => {
            print("데이터 저장 성공");
            updateItemNo(No);
            print(Player_Items[MyPlayFabInfo.PlayFabId]);
        }, (error) => print("데이터 저장 실패"));
    }

    public void updateItemNo(string No)
    {
        int itemNo = int.Parse(No);
        itemNo++;
        string UpdatedNo = itemNo.ToString();

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>() { { "ItemNo", UpdatedNo } },
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(request, (result) => {
            print("데이터 저장 성공");
            Player_Items[MyPlayFabInfo.PlayFabId] = UpdatedNo;
        }, (error) => print("데이터 저장 실패"));
    }

    public void getData()
    {
        GameObject g;
        ItemTemplate = ShopScrollView.GetChild(0).gameObject;


        for (int i = 0; i < PlayFabUserList.Count; i++)
        {
            string seller = PlayFabUserList[i].DisplayName;
            print(seller);
            var request = new GetUserDataRequest() { PlayFabId = PlayFabUserList[i].PlayFabId };
            PlayFabClientAPI.GetUserData(request, (result) => {
                foreach (var eachData in result.Data)
                {
                    if (!eachData.Value.Value.All(char.IsDigit))
                    {
                        string[] datas = eachData.Value.Value.Split('`');

                        g = Instantiate(ItemTemplate, ShopScrollView);

                        Text[] data_text = g.GetComponentsInChildren<Text>();
                        print(seller);
                        data_text[0].text = seller;
                        data_text[1].text = datas[0];
                        data_text[2].text = datas[1];
                        data_text[3].text = datas[2];

                        g.SetActive(true);
                    }
                }
            }, (error) => print("실패"));

        }
    }

    void OnLoginSuccess(LoginResult result)
    {
        print("로그인 성공");
        Flag = true;
        LoginObject.SetActive(false);

        //온라인 연결 수행. 연결이 성공하면 콜백 메소드인 OnConnectedToMaster가 실행
        if (Flag == true)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        GetLeaderboard(result.PlayFabId);

    }








    void OnLoginFailure(PlayFabError error) => print("로그인 실패");



    void OnRegisterFailure(PlayFabError error) => print("회원가입 실패");

    //void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        //생성된 방에 랜덤하게 접속한다. 
        //접속 실패 시, 콜백 메소드인 OnJoinRandomFailed가 실행되고 PhotonNetwork.CreateRoom을 통해 새로운 방을 생성한다. 
        //접속 성공 시, OnJoinedRoom가 실행되고, PhotonNetwork.Instantiate을 통해 플레이어를 생성한다.
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        //PhotonNetwork.Instantiate("Jammo_Player", Cube.transform.position, Quaternion.identity);
        PhotonNetwork.Instantiate("Player3", Cube.transform.position, Quaternion.identity);
        Debug.Log("Joined");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        this.CreateRoom();
        Debug.Log("Failed! Create new room");
    }

    void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }
}