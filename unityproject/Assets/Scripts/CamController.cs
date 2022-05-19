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
    [SerializeField]
    private Transform noticePop;

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

    // 전체 공지를 받아오고
    // 그 중 한개의 공지를 표시하는 메소드
    public void OnClickNoticeList()
    {
        BackendReturnObject BRO = Backend.Notice.NoticeList();

        if (BRO.IsSuccess())
        {
            // 전체 공지 리스트
            Debug.Log(BRO.GetReturnValue());


            // 전체 공지 중에 2번째 공지를 저장합니다.
            JsonData noticeData = BRO.GetReturnValuetoJSON()["rows"][1];

            noticePop.Find("title").GetComponentInChildren<Text>().text = noticeData["title"][0].ToString();
            noticePop.Find("content").GetComponentInChildren<Text>().text = noticeData["content"][0].ToString();

            // 이미지 참조하기
            if (noticeData["imageKey"][0] != null)
            {
                StartCoroutine(WWWImageDown("http://upload-console.thebackend.io" + noticeData["imageKey"][0]));
            }

            else
            {
                noticePop.gameObject.SetActive(true);
            }

            // 공지 게시 일자
            noticePop.Find("postingDateText").GetComponentInChildren<Text>().text = noticeData["postingDate"][0].ToString();

            // 버튼 링크 주소
            noticePop.Find("Button").GetComponent<Button>().onClick.AddListener(() => LinkWindow(noticeData["linkUrl"][0].ToString()));

            // 버튼 이름
            noticePop.Find("Button").GetComponentInChildren<Text>().text = noticeData["linkButtonName"][0].ToString();
        }

        else
        {
            Debug.Log("서버 공통 에러 발생: " + BRO.GetErrorCode());
        }
    }

    // 버튼의 클릭이벤트 (버튼 클릭 한 경우 웹 페이지 열리게 작성)
    void LinkWindow(string url)
    {
        Application.OpenURL(url);
    }

    // 이미지 받아오기
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
                Texture2D t = texDl.texture;
                Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);
                noticePop.Find("Image").GetComponent<Image>().sprite = s;

            }

            noticePop.gameObject.SetActive(true);
        }

        else
        {
            Debug.LogError(wr.error);
            Debug.Log("공지 사항을 받아오지 못했습니다");
        }
    }
}