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
[CustomEditor(typeof(UnitTable))]
public class UnitTableEditor : BaseGoogleEditor<UnitTable>
{	    
    public override bool Load()
    {        
        UnitTable targetData = target as UnitTable;
        
        var client = new DatabaseClient("", "");
        string error = string.Empty;
        var db = client.GetDatabase(targetData.SheetName, ref error);	
        var table = db.GetTable<UnitTableData>(targetData.WorksheetName) ?? db.CreateTable<UnitTableData>(targetData.WorksheetName);
        
        List<UnitTableData> myDataList = new List<UnitTableData>();
        
        var all = table.FindAll();
        foreach(var elem in all)
        {
            UnitTableData data = new UnitTableData();
            
            data = Cloner.DeepCopy<UnitTableData>(elem.Element);
            myDataList.Add(data);
        }
                
        targetData.dataArray = myDataList.ToArray();
        
        EditorUtility.SetDirty(targetData);
        AssetDatabase.SaveAssets();
        
        return true;
    }
}
