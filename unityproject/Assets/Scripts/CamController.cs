using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BackEnd;
using UnityEngine.UI;
using LitJson;
using UnityEngine.Networking;

public class CamController : MonoBehaviour
{
    // 전역 변수
    [Header("Game Manager")]
    public Transform tempNotice;
    public Transform Notice;
    string linkURL;

    public GameObject player; // 바라볼 플레이어 오브젝트입니다.
    public float xmove = 2;  // X축 누적 이동량
    public float ymove = 25;  // Y축 누적 이동량
    public float distance = 1;

    private Vector3 velocity = Vector3.zero;

    private int toggleView = 3; // 1=1인칭, 3=3인칭

    private float wheelspeed = 10.0f;

    private Vector3 Player_Height;
    private Vector3 Player_Side;

    void Start()
    {
        Player_Height = new Vector3(0f, 14f, 0f);
        Player_Side = new Vector3(-3f, -7f, -10f);
    }

    // Update is called once per frame
    void Update()
    {
        // 종료 버튼 누른 경우 로그아웃 위한 코드
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("LogoutScene");
        }

        //  마우스 우클릭 중에만 카메라 무빙 적용
        if (Input.GetMouseButton(1))
        {
            xmove += Input.GetAxis("Mouse X"); // 마우스의 좌우 이동량을 xmove 에 누적합니다.
            ymove -= Input.GetAxis("Mouse Y"); // 마우스의 상하 이동량을 ymove 에 누적합니다.
        }
        transform.rotation = Quaternion.Euler(ymove, xmove, 0); // 이동량에 따라 카메라의 바라보는 방향을 조정합니다.

        if (Input.GetMouseButtonDown(2))
            toggleView = 4 - toggleView;

        if (toggleView == 3)
        {
            distance -= Input.GetAxis("Mouse ScrollWheel") * wheelspeed;
            if (distance < 1f) distance = 1f;
            if (distance > 100.0f) distance = 100.0f;
        }

        if (toggleView == 1)
        {
            Vector3 Eye = player.transform.position + Player_Height;
            Vector3 reverseDistance = new Vector3(-2.0f, -2.0f, 4.0f); // 카메라가 바라보는 앞방향은 Z 축입니다. 이동량에 따른 Z 축방향의 벡터를 구합니다.
            transform.position = Eye + transform.rotation * reverseDistance; // 플레이어의 위치에서 카메라가 바라보는 방향에 벡터값을 적용한 상대 좌표를 차감합니다.
        }
        else if (toggleView == 3)
        {
            Vector3 Eye = player.transform.position + transform.rotation * Player_Side + Player_Height;
            Vector3 reverseDistance = new Vector3(0.0f, 0.0f, distance); // 카메라가 바라보는 앞방향은 Z 축입니다. 이동량에 따른 Z 축방향의 벡터를 구합니다.
            transform.position = Eye - transform.rotation * reverseDistance;           
        }
    }

    public void getNotice()
    {
        BackendReturnObject BRO = Backend.Notice.NoticeList();

        if (BRO.IsSuccess())
        {

            JsonData noticeData = BRO.GetReturnValuetoJSON()["rows"][0];

            string date = noticeData["postingDate"][0].ToString();
            string title = noticeData["title"][0].ToString();
            string content = noticeData["content"][0].ToString().Substring(0, 10);
            string imgURL = "http://upload-console.thebackend.io" + noticeData["imageKey"][0];
            linkURL = noticeData["linkUrl"][0].ToString();

            Notice.GetChild(6).GetComponent<Text>().text = date;
            Notice.GetChild(5).GetComponent<Text>().text = title;
            Notice.GetChild(7).GetComponent<Text>().text = content;
            StartCoroutine(WWWImageDown(imgURL));

        }
    }

    IEnumerator WWWImageDown(string url)
    {
        UnityWebRequest wr = new UnityWebRequest(url);
        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        wr.downloadHandler = texDl;
        yield return wr.SendWebRequest();

        if (!(wr.isNetworkError || wr.isHttpError))
        {
            if (texDl.texture != null)
            {
                print("이미지 로드 완료");
                Texture2D t = texDl.texture;
                Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);
                Notice.GetChild(4).GetComponent<Image>().sprite = s;
            }
        }
        else
        {
            print("이미지가 없습니다.");
        }

        Notice.gameObject.SetActive(true);
    }
}