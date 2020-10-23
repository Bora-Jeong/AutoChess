using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PacketManager : HNET.CHNetConnector
{
    public enum PacketType
    {
        Login,
        Authentication,
        GetRanking
    }

    private static PacketManager _instance;
    public static PacketManager instance
    {
        get
        {
            if (_instance == null)
            {
                PacketManager[] objectList = Resources.FindObjectsOfTypeAll<PacketManager>();
                if (objectList.Length > 0)
                    (objectList[0] as PacketManager).Awake();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        StartCoroutine(ConnectToServer());
    }

    private IEnumerator ConnectToServer()
    {
        Connect();

        yield return new WaitForSeconds(0.3f);

        if (!IsConnect())
        {
            PopUpPanel.instance.PopUpReconnect(() =>
            {
                StartCoroutine(ConnectToServer());
            });
        }
    }

    public override void OnConnect()
    {
        Debug.Log("Connet");
    }

    public override void OnDisconnect()
    {
        Debug.Log("Disconnet");
    }


    public override void OnMessage(HNetPacket Packet)
    {
        switch (Packet.Type())
        {
            case (int)PacketType.Login:
                GetPacket_Login(Packet);
                break;

            case (int)PacketType.Authentication:
                GetPacket_Authentication(Packet);
                break;

            case (int)PacketType.GetRanking:
                LobbyPanel.instance.RefreshRanking(Packet);
                break;

            default:
                Disconnect();
                break;
        }
    }

    public void GetServerData()
    {
        // 랭킹 요청
        HNET.NewPacket Out = new HNET.NewPacket((int)PacketType.GetRanking);
        Send(Out);
    }

    public void SendPacket_Login(string nickname, string password) // 로그인 요청
    {
        HNET.NewPacket Out = new HNET.NewPacket((int)PacketType.Login);
        Out.In(nickname, Encoding.Unicode);
        Out.In(password, Encoding.Unicode);
        Send(Out);
    }

    private void GetPacket_Login(HNetPacket Packet) // 로그인 요청 결과
    {
        bool isSuccess = false;
        Packet.Out(ref isSuccess);

        if (isSuccess) // 로그인 성공
        {
            GetServerData();
            string nickname = "";
            Packet.Out(ref nickname, Encoding.Unicode);
            Player.instance.nickname = nickname;
            LoginPanel.instance.SaveCurrentInfo();
            LoginPanel.instance.Hide();
            LoadingPanel.instance.Show();
            LoadingPanel.instance.LoadAllUnit();
        }
        else
        {
            PopUpPanel.instance.PopUpNotice("아이디 혹은 비밀번호가 잘못 되었습니다.");
        }
    }

    public void SendPacket_Authentication(string nickname, string password) // 회원가입 요청
    {
        HNET.NewPacket Out = new HNET.NewPacket((int)PacketType.Authentication);
        Out.In(nickname, Encoding.Unicode);
        Out.In(password, Encoding.Unicode);
        Send(Out);
    }

    private void GetPacket_Authentication(HNetPacket Packet) // 회원가입 요청 결과
    {
        bool isSuccess = false;
        Packet.Out(ref isSuccess);
        if(isSuccess)
            PopUpPanel.instance.PopUpNotice("회원가입을 완료하였습니다.");
        else
            PopUpPanel.instance.PopUpNotice("이미 존재하는 닉네임입니다.");
    }
}
