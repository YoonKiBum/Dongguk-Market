using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogoutScript : MonoBehaviour
{
    // ���� ������ ����
    /*
    [Header("Login & Register")]
    public InputField ID;
    public InputField PW;
    */

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

    // ���� ��� �α׾ƿ�
    public void LogOut()
    {
        Backend.BMember.Logout();
        //ID.text = PW.text = "";
        print("���� ��� �α׾ƿ� ����");
        SceneManager.LoadScene("InitScene");
    }
    
    // �ƴϿ� ��ư ���� ���
    public void No()
    {
        SceneManager.LoadScene("mainScene");
    }
}

