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
[CustomEditor(typeof(ClassSynergyTable))]
public class ClassSynergyTableEditor : BaseGoogleEditor<ClassSynergyTable>
{	    
    public override bool Load()
    {        
        ClassSynergyTable targetData = target as ClassSynergyTable;
        
        var client = new DatabaseClient("", "");
        string error = string.Empty;
        var db = client.GetDatabase(targetData.SheetName, ref error);	
        var table = db.GetTable<ClassSynergyTableData>(targetData.WorksheetName) ?? db.CreateTable<ClassSynergyTableData>(targetData.WorksheetName);
        
        List<ClassSynergyTableData> myDataList = new List<ClassSynergyTableData>();
        
        var all = table.FindAll();
        foreach(var elem in all)
        {
            ClassSynergyTableData data = new ClassSynergyTableData();
            
            data = Cloner.DeepCopy<ClassSynergyTableData>(elem.Element);
            myDataList.Add(data);
        }
                
        targetData.dataArray = myDataList.ToArray();
        
        EditorUtility.SetDirty(targetData);
        AssetDatabase.SaveAssets();
        
        return true;
    }
}
