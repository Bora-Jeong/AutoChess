using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class TableData : Singleton<TableData>
{
    [SerializeField]
    private UnitTable _unitTable;
    [SerializeField]
    private StringTable _stringTable;
    [SerializeField]
    private ClassSynergyTable _classSynergyTable;
    [SerializeField]
    private SpeciesSynergyTable _speciesSynergyTable;

    private Dictionary<int, UnitTableData> _unitTableDic;
    private Dictionary<string, StringTableData> _stringTableDic;
    private Dictionary<Clas, ClassSynergyTableData> _classSynergyTableDic;
    private Dictionary<Species, SpeciesSynergyTableData> _speciesSynergyTableDic;

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

        _stringTableDic = new Dictionary<string, StringTableData>();
        for (int i = 0; i < _stringTable.dataArray.Length; i++)
            _stringTableDic.Add(_stringTable.dataArray[i].Typeid, _stringTable.dataArray[i]);

        _classSynergyTableDic = new Dictionary<Clas, ClassSynergyTableData>();
        for (int i = 0; i < _classSynergyTable.dataArray.Length; i++)
            _classSynergyTableDic.Add(_classSynergyTable.dataArray[i].CLAS, _classSynergyTable.dataArray[i]);

        _speciesSynergyTableDic = new Dictionary<Species, SpeciesSynergyTableData>();
        for (int i = 0; i < _speciesSynergyTable.dataArray.Length; i++)
            _speciesSynergyTableDic.Add(_speciesSynergyTable.dataArray[i].SPECIES, _speciesSynergyTable.dataArray[i]);
    }

    public UnitTableData GetUnitTableData(int unitID)
    {
        if (_unitTableDic.TryGetValue(unitID, out UnitTableData data))
            return data;
        Debug.LogError($"UnitID가 {unitID} 인 데이터가 UnitTable에 존재하지 않습니다.");
        return null;
    }

    public string GetStringTableData(string typeID)
    {
        if (_stringTableDic.TryGetValue(typeID, out StringTableData data))
            return data.Content;
        Debug.LogError($"TypeID가 {typeID} 인 데이터가 StringTable에 존재하지 않습니다.");
        return string.Empty;
    }

    public ClassSynergyTableData GetClassSynergyTableData(Clas clas)
    {
        if (_classSynergyTableDic.TryGetValue(clas, out ClassSynergyTableData data))
            return data;
        Debug.LogError($"TypeID가 {clas} 인 데이터가 ClassSynergyTable에 존재하지 않습니다.");
        return null;
    }

    public SpeciesSynergyTableData GetSpeciesSynergyTableData(Species species)
    {
        if (_speciesSynergyTableDic.TryGetValue(species, out SpeciesSynergyTableData data))
            return data;
        Debug.LogError($"TypeID가 {species} 인 데이터가 SpeciesSynergyTable에 존재하지 않습니다.");
        return null;
    }

}