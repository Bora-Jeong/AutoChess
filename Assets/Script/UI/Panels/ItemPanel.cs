using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour
{
    [SerializeField]
    private Image _buttonImage;
    [SerializeField]
    private Sprite[] _buttonSprites;
    [SerializeField]
    private Item _item;
    [SerializeField]
    private Transform _itemRoot;
    [SerializeField]
    private Transform _invenItemRoot;
    [SerializeField]
    private InputManager _inputManager;
    [SerializeField]
    private MouseCursor _mouseCursor;

    private RectTransform _rectTransform;
    private float _width;
    private bool _isOn;
    private Dictionary<int, Item> _inventory;

    private readonly float moveTime = 0.4f;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _width = _rectTransform.rect.width;
        _inventory = new Dictionary<int, Item>();
    }

    public void OnItemShopButton()
    {
        LeanTween.value(gameObject, _rectTransform.anchoredPosition.x, _isOn ? _width : 0, moveTime).setOnUpdate((float x) =>
        {
            _rectTransform.anchoredPosition = new Vector2(x, _rectTransform.anchoredPosition.y);
        });
        _isOn = !_isOn;
        _buttonImage.sprite = _buttonSprites[_isOn ? 1 : 0];
    }

    public void Clear()
    {
        
    }

    public void LoadItems()
    {
        var dic = TableData.instance.itemTableDic;
        foreach(var pair in dic)
        {
            Item item = Instantiate(_item, _itemRoot);
            item.Set(pair.Value, false);
        }
    }

    public void BuyItem(ItemTableData data)
    {
        if (Player.instance.gold < data.Gold)
        {
            SoundManager.PlaySFX("nogold");
            return;
        }

        SoundManager.PlaySFX("gold");
        Player.instance.gold -= data.Gold;
        if (_inventory.TryGetValue(data.Typeid, out Item myItem)) // 이미 있던 아이템
            myItem.count++;
        else
        {
            Item newItem = Instantiate(_item, _invenItemRoot);
            newItem.Set(data, true);
            newItem.count = 1;
            _inventory.Add(data.Typeid, newItem);
        }
    }

    public void UseItem(Item item)
    {
        switch (item.data.USETYPE)
        {
            case UseType.Player:
                UsePlayerItem(item);
                break;

            case UseType.Unit:
                UseUnitItem(item);
                break;
        }
    }

    private void UsePlayerItem(Item item)
    {
        ItemTableData data = item.data;
        switch (data.Typeid)
        {
            case 100: // 체력 증가
                Player.instance.hp = Mathf.Min(Player.instance.hp + data.Value, 100);
                break;

            case 103: //랜덤으로 최소 1골드 최대 10골드를 얻습니다.
                Player.instance.gold += Random.Range(1, 11);
                break;
        }
        ItemCountDown(item);
    }

    private void UseUnitItem(Item item)
    {
        ItemTableData data = item.data;
        _mouseCursor.SetCursor(Item.GetItemIcon(data.Typeid));
        _inputManager.GrabItem((Unit unit) =>
        {
            if (unit != null)
            {
                switch (data.Typeid)
                {
                    case 101: // 공격력 증가
                        unit.curAttackPower += data.Value;
                        break;

                    case 102: // 공격력 증가
                        unit.curDeffensePower += data.Value;
                        break;

                    case 104: // 유닛의 스킬 쿨타임이 {0}% 감소합니다.
                        unit.curSkillCoolTime -= unit.Data.Skillcooltime * data.Value / 100;
                        break;

                    case 105: //유닛의 최대 체력이 {0} 증가합니다.
                        unit.curFullHp += data.Value;
                        break;

                    case 106: //유닛의 공격 속도가 {0} 상승합니다.
                        unit.curAttackTerm -= unit.Data.Attackterm * data.Value / 100;
                        break;

                    case 107: //유닛의 이동 속도가 {0}% 상승합니다.
                        unit.curMoveSpeed += unit.Data.Movespeed * data.Value / 100;
                        break;
                }

                ItemCountDown(item);
            }
            _mouseCursor.SetCursor();
        });
    }

    private void ItemCountDown(Item item)
    {
        item.count--;
        if (item.count <= 0)
        {
            Destroy(item.gameObject);
            _inventory.Remove(item.data.Typeid);
        }
    }
}
