using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogoutScript : MonoBehaviour
{
    // 전역 변수로 선언
    /*
    [Header("Login & Register")]
    public InputField ID;
    public InputField PW;
    */

    void Start()
    {
        // 초기화
        // [.net4][il2cpp] 사용 시 필수 사용
        Backend.Initialize(() =>
        {
            // 초기화 성공한 경우 실행
            if (Backend.IsInitialized)
            {
                print("뒤끝 초기화 성공");
                // example
                // 버전체크 -> 업데이트
            }
            // 초기화 실패한 경우 실행
            else
            {
                print("뒤끝 초기화 실패");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 동기 방식 로그아웃
    public void LogOut()
    {
        Backend.BMember.Logout();
        //ID.text = PW.text = "";
        print("동기 방식 로그아웃 성공");
        SceneManager.LoadScene("InitScene");
    }
    
    // 아니요 버튼 누른 경우
    public void No()
    {
        SceneManager.LoadScene("mainScene");
    }
}

