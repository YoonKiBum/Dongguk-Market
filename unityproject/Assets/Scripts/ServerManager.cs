using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ServerManager : MonoBehaviour
{
    // 전역 변수로 선언
    [Header("Login & Register")]
    public InputField ID;
    public InputField PW;

    // 에러 코드 확인
    void Error(string errorCode)
    {
        switch (errorCode)
        {
            case "DuplicatedParameterException":
                print("중복된 사용자 아이디 입니다.");
                break;
            case "BadUnauthorizedException":
                print("잘못된 사용자 아이디 혹은 비밀번호 입니다.");
                break;
            default:
                break;
        }
    }

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

    // 동기방식 회원가입
    public void Register()
    {
        BackendReturnObject BRO = Backend.BMember.CustomSignUp(ID.text, PW.text);

        if (BRO.IsSuccess()) print("동기방식 회원가입 성공");
        else Error(BRO.GetErrorCode());

    }

    // 동기방식 로그인
    public void Login()
    {
        BackendReturnObject BRO = Backend.BMember.CustomLogin(ID.text, PW.text);

        if (BRO.IsSuccess())
        {
            print("동기방식 로그인 성공");
            SceneManager.LoadScene("mainScene");
        }
        else Error(BRO.GetErrorCode());
    }
}

