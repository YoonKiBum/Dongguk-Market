
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
    public InputField NameInput, PriceInput, InfoInput, SearchInput;

    public Text StatusText;//접속상태
                           // public InputField NickNameInput;//닉네임
    public GameObject Cube;//플레이어 생성 위치
    public string gameVersion = "1.0";//게임 버전

    public GameObject shop; // 구매 UI
    public GameObject register; // 판매 UI

    public static int MAX_PLAYER = 10; // 리더 보드에서 가져올 최대 플레이어의 수
   
    public PlayerLeaderboardEntry MyPlayFabInfo; // 리더 보드에서 가져온 Current Player의 Info
    public List<PlayerLeaderboardEntry> PlayFabUserList = new List<PlayerLeaderboardEntry>(); // 리더 보드에서 가져온 모든 Player의 Info

    [SerializeField] Transform ShopScrollView; //S hop에 존재하는 Item들을 담고 있는 ScrollView
    public List<GameObject> Shop_Items = new List<GameObject>(); // ScrollView 내에 생성되는 모든 Item들을 담는 List
    public GameObject Sample_Item; // 상점에 인스턴스 생성하기 위해 만들어 놓은 sample -> setActive(false) 하기 위함
    GameObject ItemTemplate; //Shop에 존재하는 Item 템플릿 -> Sample_Item을 템플릿으로
    public Dictionary<string, string> Player_Items = new Dictionary<string, string>(); // 각 플레이어가 등록한 Items의 Number

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


    public void RegisterBtn() // 회원가입 버튼
    {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text, DisplayName = UsernameInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => { print("회원가입 성공"); SetStat(); SetData("0"); }, (error) => print("회원가입 실패"));
    }

    public void UploadBtn() // 판매 Upload 버튼
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

    public void SearchBtn() // 구매 Search 버튼
    {
        if (Shop_Items.Count != 0)
        {
            foreach (GameObject game in Shop_Items)
            {
                Destroy(game);
            }
            Shop_Items.Clear();
        }

        GameObject g;
        ItemTemplate = ShopScrollView.GetChild(0).gameObject;

        print(SearchInput.text);
        for (int i = 0; i < PlayFabUserList.Count; i++)
        {
            string seller = PlayFabUserList[i].DisplayName;

            var request = new GetUserDataRequest() { PlayFabId = PlayFabUserList[i].PlayFabId };
            PlayFabClientAPI.GetUserData(request, (result) => {
                foreach (var eachData in result.Data)
                {
                    if (!eachData.Value.Value.All(char.IsDigit))
                    {
                        string[] datas = eachData.Value.Value.Split('`');

                        g = Instantiate(ItemTemplate, ShopScrollView);

                        Text[] data_text = g.GetComponentsInChildren<Text>();
                        
                        data_text[0].text = seller;
                        data_text[1].text = datas[0];
                        data_text[2].text = datas[1];
                        data_text[3].text = datas[2];

                        g.SetActive(true);
                        Shop_Items.Add(g);
                    }
                }
            }, (error) => print("실패"));

        }
    }

    public void purchaseCloseBtn() // 구매 Close 버튼
    {
        int children = ShopScrollView.childCount;

        for (int i = 0; i < children; i++)
        {
            ShopScrollView.GetChild(i).gameObject.SetActive(false);
        }

        shop.SetActive(false);
    }

    void GetLeaderboard(string myID) // 리더보드 내의 Player Info를 받아 저장
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
                    string k = result.Leaderboard[j].PlayFabId;
                    Player_Items.Add(k, "0");
                    if (result.Leaderboard[j].PlayFabId == myID) MyPlayFabInfo = result.Leaderboard[j];
                }

                getUserItemNo();
            },
            (error) => { });
        }
    }

    void SetStat() // 회원가입 시 리더보드에 Player Data 생성
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "IDInfo", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => print("값 저장실패"));
    }

    void SetData(string initNo) // 회원가입 시 Player가 등록한 Item Number를 0으로 생성
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>() { { "ItemNo", initNo } },
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(request, (result) => print("데이터 저장 성공"), (error) => print("데이터 저장 실패"));
    }

    public void getUserItemNo() // 존재하는 모든 Player가 현재 등록한 각 Item의 수를 저장
    {

        for (int i = 0; i < PlayFabUserList.Count; i++)
        {
            string Id = PlayFabUserList[i].PlayFabId;
            var request = new GetUserDataRequest() { PlayFabId = PlayFabUserList[i].PlayFabId };
            PlayFabClientAPI.GetUserData(request,
                (result) => {
                    if (result.Data == null || !result.Data.ContainsKey("ItemNo")) Debug.Log("No Data");
                    else if(result.Data.ContainsKey("ItemNo"))
                    {
                        string Value = result.Data["ItemNo"].Value;
                        
                        if (!Player_Items.ContainsKey(Id))
                        {
                            Player_Items.Add(Id, Value);
                        }
                        else
                        {
                            Player_Items[Id] = Value;
                        }
                    }
                }, (error) => {
                    Debug.Log("Got error retrieving user data");
                });
        }
    }


    public void updateItemNo(string No) // Player가 물건 Upload 시 Item의 Number Update
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