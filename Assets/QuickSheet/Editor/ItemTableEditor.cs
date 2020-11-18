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
[CustomEditor(typeof(ItemTable))]
public class ItemTableEditor : BaseGoogleEditor<ItemTable>
{	    
    public override bool Load()
    {        
        ItemTable targetData = target as ItemTable;
        
        var client = new DatabaseClient("", "");
        string error = string.Empty;
        var db = client.GetDatabase(targetData.SheetName, ref error);	
        var table = db.GetTable<ItemTableData>(targetData.WorksheetName) ?? db.CreateTable<ItemTableData>(targetData.WorksheetName);
        
        List<ItemTableData> myDataList = new List<ItemTableData>();
        
        var all = table.FindAll();
        foreach(var elem in all)
        {
            ItemTableData data = new ItemTableData();
            
            data = Cloner.DeepCopy<ItemTableData>(elem.Element);
            myDataList.Add(data);
        }
                
        targetData.dataArray = myDataList.ToArray();
        
        EditorUtility.SetDirty(targetData);
        AssetDatabase.SaveAssets();
        
        return true;
    }
}
