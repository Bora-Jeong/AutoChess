using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SynergyItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image _iconImage;
    [SerializeField]
    private Text _unitCountText;
    [SerializeField]
    private Text _nameText;
    [SerializeField]
    private Text[] _synergyListTexts;
    [SerializeField]
    private GameObject _descGroup;
    [SerializeField]
    private Text _descText;

    public void SetUp(Clas clas, int unitCount)
    {
        _iconImage.sprite = FieldManager.instance.GetClassIcon(clas);
        _nameText.text = clas.ToName();
        _unitCountText.text = unitCount.ToString();
        _descText.text = string.Empty;
        var data = TableData.instance.GetClassSynergyTableData(clas);
        int textCount = 0;
        for(int i = 0; i < data.Typecount.Length; i++)
        {
            if (i == 0)
            {
                SetText(_synergyListTexts[i], data.Typecount[i].ToString(), unitCount >= data.Typecount[i]);
                textCount++;
            }
            else
            {
                SetText(_synergyListTexts[2 * i - 1], " / ", unitCount >= data.Typecount[i]);
                SetText(_synergyListTexts[2 * i], data.Typecount[i].ToString(), unitCount >= data.Typecount[i]);
                textCount += 2;
            }
            if (data.Values.Length > i)
                _descText.text += $"{data.Typecount[i]} : " + string.Format($"synergyDesc_{clas}".Convert(), data.Values[i]) + "\n";
        }
        for (int i = textCount; i < _synergyListTexts.Length; i++)
            _synergyListTexts[i].gameObject.SetActive(false);
        if (clas == Clas.Dragon) _descText.text = $"synergyDesc_{clas}".Convert(); // 용은 예외
        _descGroup.SetActive(false);
    }

    public void SetUp(Species species, int unitCount)
    {
        _iconImage.sprite = FieldManager.instance.GetSpeciesIcon(species);
        _unitCountText.text = unitCount.ToString();
        _nameText.text = species.ToName();
        _descText.text = string.Empty;
        var data = TableData.instance.GetSpeciesSynergyTableData(species);
        int textCount = 0;
        for (int i = 0; i < data.Typecount.Length; i++)
        {
            if (i == 0)
            {
                SetText(_synergyListTexts[i], data.Typecount[i].ToString(), unitCount >= data.Typecount[i]);
                textCount++;
            }
            else
            {
                SetText(_synergyListTexts[2 * i - 1], " / ", unitCount >= data.Typecount[i]);
                SetText(_synergyListTexts[2 * i], data.Typecount[i].ToString(), unitCount >= data.Typecount[i]);
                textCount += 2;
            }
            if (data.Values.Length > i)
                _descText.text += $"{data.Typecount[i]} : " + string.Format($"synergyDesc_{species}".Convert(), data.Values[i]) + "\n";
        }
        for (int i = textCount; i < _synergyListTexts.Length; i++)
            _synergyListTexts[i].gameObject.SetActive(false);
        _descGroup.SetActive(false);
    }

    private void SetText(Text text, string str, bool active)
    {
        text.gameObject.SetActive(true);
        text.text = str;
        text.color = active ? Color.white : Color.grey;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _descGroup.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _descGroup.SetActive(false);
    }
}
