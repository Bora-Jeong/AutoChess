using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TableData : Singleton<TableData>
{
    [SerializeField]
    private UnitTable _unitTable;
    [SerializeField]
    private SkillDescTable _skillDescTable;

    private Dictionary<int, UnitTableData> _unitTableDic;
    private Dictionary<int, SkillDescTableData> _skillDescTableDic;

    public Dictionary<int, UnitTableData> unitTableDic => _unitTableDic;
    private void Awake()
    {
        LoadTableData();
    }

    private void LoadTableData()
    {
        _unitTableDic = new Dictionary<int, UnitTableData>();
        for (int i = 0; i < _unitTable.dataArray.Length; i++)
            _unitTableDic.Add(_unitTable.dataArray[i].Unitid, _unitTable.dataArray[i]);

        _skillDescTableDic = new Dictionary<int, SkillDescTableData>();
        for (int i = 0; i < _skillDescTable.dataArray.Length; i++)
            _skillDescTableDic.Add(_skillDescTable.dataArray[i].Unitid, _skillDescTable.dataArray[i]);
    }

    public UnitTableData GetUnitTableData(int unitID)
    {
        if (_unitTableDic.TryGetValue(unitID, out UnitTableData data))
            return data;
        Debug.Log($"UnitID가 {unitID} 인 데이터가 UnitTable에 존재하지 않습니다.");
        return null;
    }

    public SkillDescTableData GetSkillDescTableData(int unitID)
    {
        if (_skillDescTableDic.TryGetValue(unitID, out SkillDescTableData data))
            return data;
        Debug.Log($"UnitID가 {unitID} 인 데이터가 SkillDescTable 존재하지 않습니다.");
        return null;
    }

}