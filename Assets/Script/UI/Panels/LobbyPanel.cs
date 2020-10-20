using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : PanelBase<LobbyPanel>
{
    [SerializeField]
    private GameObject[] _borders;
    [SerializeField]
    private Transform _rankingItemRoot;
    [SerializeField]
    private RankingItem _rankingItem;

    public override void Init()
    {
        SetTab(0);
    }

    public void RefreshRanking(HNetPacket Packet)
    {
        RankingItem[] items = _rankingItemRoot.GetComponentsInChildren<RankingItem>();
        for (int i = 0; i < items.Length; i++)
            Destroy(items[i].gameObject);

        int rank = 1;
        while (!Packet.Empty())
        {
            string nickname = "";
            int win = 0, lose = 0;
            float rate = 0f;
            Packet.Out(ref nickname, Encoding.Unicode);
            Packet.Out(ref win);
            Packet.Out(ref lose);
            Packet.Out(ref rate);
            RankingItem item = Instantiate(_rankingItem, _rankingItemRoot);
            item.SetUp(rank, nickname, win, lose, rate);
            rank++;
        }

    }

    public void OnStartButtonClick()
    {
        GameManager.instance.StartGame();
    }   
    private void SetTab(int tab)
    {
        for (int i = 0; i < _borders.Length; i++)
            _borders[i].SetActive(i == tab);
    }

    public void OnRankingTabClick()
    {
        SetTab(0);
    }

    public void OnUnitTabClick()
    {
        SetTab(1);
    }

    public void OnInfoTabClick()
    {
        SetTab(2);
    }
}
