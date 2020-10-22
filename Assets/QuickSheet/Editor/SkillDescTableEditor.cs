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
[CustomEditor(typeof(SkillDescTable))]
public class SkillDescTableEditor : BaseGoogleEditor<SkillDescTable>
{	    
    public override bool Load()
    {        
        SkillDescTable targetData = target as SkillDescTable;
        
        var client = new DatabaseClient("", "");
        string error = string.Empty;
        var db = client.GetDatabase(targetData.SheetName, ref error);	
        var table = db.GetTable<SkillDescTableData>(targetData.WorksheetName) ?? db.CreateTable<SkillDescTableData>(targetData.WorksheetName);
        
        List<SkillDescTableData> myDataList = new List<SkillDescTableData>();
        
        var all = table.FindAll();
        foreach(var elem in all)
        {
            SkillDescTableData data = new SkillDescTableData();
            
            data = Cloner.DeepCopy<SkillDescTableData>(elem.Element);
            myDataList.Add(data);
        }
                
        targetData.dataArray = myDataList.ToArray();
        
        EditorUtility.SetDirty(targetData);
        AssetDatabase.SaveAssets();
        
        return true;
    }
}
