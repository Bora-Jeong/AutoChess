using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField]
    private Image _iconImage;
    [SerializeField]
    private Text _nameText;
    [SerializeField]
    private Text _descText;
    [SerializeField]
    private Text _goldText;
    [SerializeField]
    private GameObject _goldIcon;
    [SerializeField]
    private GameObject _countText;
    [SerializeField]
    private Button _button;

    private bool _isInvenItem;
    private ItemTableData _data;
    public ItemTableData data => _data;

    public void Set(ItemTableData itemData, bool isInvenItem)
    {
        _data = itemData;
        _isInvenItem = isInvenItem;
        _iconImage.sprite = GetItemIcon( _data.Typeid);
        _nameText.text = _data.Name;
        _descText.text = string.Format(TableData.instance.GetStringTableData($"ItemDesc_{_data.Typeid}"), _data.Value);
        _goldText.text = _data.Gold.ToString();
        _goldIcon.SetActive(!isInvenItem);
        _countText.SetActive(isInvenItem);
        if (_isInvenItem)  _button.onClick.AddListener(Use);
        else _button.onClick.AddListener(Buy);
    }

    private int _count;
    public int count
    {
        get => _count;
        set
        {
            _count = value;
            _goldText.text = _count.ToString();
        }
    }

    private void Buy()
    {
        GamePanel.instance.itemPanel.BuyItem(_data);
    }

    private void Use()
    {
        SoundManager.PlaySFX("item");
        GamePanel.instance.itemPanel.UseItem(this);
    }

    public static Sprite GetItemIcon(int typeID) =>  Resources.Load<Sprite>($"ItemIcon/{typeID}");

}
