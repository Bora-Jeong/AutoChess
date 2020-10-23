using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using GDataDB;
using GDataDB.Linq;

using UnityQuickSheet;

///
/// !!! Machine generated code !!!
///
[CustomEditor(typeof(SpeciesSynergyTable))]
public class SpeciesSynergyTableEditor : BaseGoogleEditor<SpeciesSynergyTable>
{	    
    public override bool Load()
    {        
        SpeciesSynergyTable targetData = target as SpeciesSynergyTable;
        
        var client = new DatabaseClient("", "");
        string error = string.Empty;
        var db = client.GetDatabase(targetData.SheetName, ref error);	
        var table = db.GetTable<SpeciesSynergyTableData>(targetData.WorksheetName) ?? db.CreateTable<SpeciesSynergyTableData>(targetData.WorksheetName);
        
        List<SpeciesSynergyTableData> myDataList = new List<SpeciesSynergyTableData>();
        
        var all = table.FindAll();
        foreach(var elem in all)
        {
            SpeciesSynergyTableData data = new SpeciesSynergyTableData();
            
            data = Cloner.DeepCopy<SpeciesSynergyTableData>(elem.Element);
            myDataList.Add(data);
        }
                
        targetData.dataArray = myDataList.ToArray();
        
        EditorUtility.SetDirty(targetData);
        AssetDatabase.SaveAssets();
        
        return true;
    }
}
