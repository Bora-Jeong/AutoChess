using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitListItem : MonoBehaviour
{
    [SerializeField]
    private GameObject _selectedGroup;
    [SerializeField]
    private Text _nameText;
    [SerializeField]
    private Text _classText;
    [SerializeField]
    private Text _speciesText;
    [SerializeField]
    private Text _goldText;

    public static UnitListItem SelectedItem;

    private UnitTableData _data;
    public UnitTableData data
    {
        get => _data;
        set
        {
            _data = value;
            _nameText.text = _data.Name;
            _nameText.color = _data.Gold.GoldColor();
            _classText.text = _data.CLAS.ToName();
            _speciesText.text = _data.SPECIES.ToName();
            _goldText.text = _data.Gold.ToString();
            _goldText.color = _data.Gold.GoldColor();
        }
    }

    public void OnClick()
    {
        if (SelectedItem != null)
        {
            if (SelectedItem == this) return;
            else SelectedItem.DeSelect();
        }

        Select();
        LobbyPanel.instance.OnUnitListItemClick(_data.Unitid);
    }

    public void Select()
    {
        SelectedItem = this;
        _selectedGroup.SetActive(true);
    }

    public void DeSelect()
    {
        SelectedItem = null;
        _selectedGroup.SetActive(false);
    }
}
