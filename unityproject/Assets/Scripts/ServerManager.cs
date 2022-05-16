using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ServerManager : MonoBehaviour
{
    // ���� ������ ����
    [Header("Login & Register")]
    public InputField ID;
    public InputField PW;

    // ���� �ڵ� Ȯ��
    void Error(string errorCode)
    {
        switch (errorCode)
        {
            case "DuplicatedParameterException":
                print("�ߺ��� ����� ���̵� �Դϴ�.");
                break;
            case "BadUnauthorizedException":
                print("�߸��� ����� ���̵� Ȥ�� ��й�ȣ �Դϴ�.");
                break;
            default:
                break;
        }
    }

    void Start()
    {
        // �ʱ�ȭ
        // [.net4][il2cpp] ��� �� �ʼ� ���
        Backend.Initialize(() =>
        {
            // �ʱ�ȭ ������ ��� ����
            if (Backend.IsInitialized)
            {
                print("�ڳ� �ʱ�ȭ ����");
                // example
                // ����üũ -> ������Ʈ
            }
            // �ʱ�ȭ ������ ��� ����
            else
            {
                print("�ڳ� �ʱ�ȭ ����");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    // ������ ȸ������
    public void Register()
    {
        BackendReturnObject BRO = Backend.BMember.CustomSignUp(ID.text, PW.text);

        if (BRO.IsSuccess()) print("������ ȸ������ ����");
        else Error(BRO.GetErrorCode());

    }

    // ������ �α���
    public void Login()
    {
        BackendReturnObject BRO = Backend.BMember.CustomLogin(ID.text, PW.text);

        if (BRO.IsSuccess())
        {
            print("������ �α��� ����");
            SceneManager.LoadScene("mainScene");
        }
        else Error(BRO.GetErrorCode());
    }
}

