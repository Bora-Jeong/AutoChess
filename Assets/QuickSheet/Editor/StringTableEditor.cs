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
[CustomEditor(typeof(StringTable))]
public class StringTableEditor : BaseGoogleEditor<StringTable>
{	    
    public override bool Load()
    {        
        StringTable targetData = target as StringTable;
        
        var client = new DatabaseClient("", "");
        string error = string.Empty;
        var db = client.GetDatabase(targetData.SheetName, ref error);	
        var table = db.GetTable<StringTableData>(targetData.WorksheetName) ?? db.CreateTable<StringTableData>(targetData.WorksheetName);
        
        List<StringTableData> myDataList = new List<StringTableData>();
        
        var all = table.FindAll();
        foreach(var elem in all)
        {
            StringTableData data = new StringTableData();
            
            data = Cloner.DeepCopy<StringTableData>(elem.Element);
            myDataList.Add(data);
        }
                
        targetData.dataArray = myDataList.ToArray();
        
        EditorUtility.SetDirty(targetData);
        AssetDatabase.SaveAssets();
        
        return true;
    }
}
