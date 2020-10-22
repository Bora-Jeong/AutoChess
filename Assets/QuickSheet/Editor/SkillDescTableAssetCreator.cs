using UnityEngine;
using UnityEditor;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
/// 
public partial class GoogleDataAssetUtility
{
    [MenuItem("Assets/Create/Google/SkillDescTable")]
    public static void CreateSkillDescTableAssetFile()
    {
        SkillDescTable asset = CustomAssetUtility.CreateAsset<SkillDescTable>();
        asset.SheetName = "DataTable";
        asset.WorksheetName = "SkillDescTable";
        EditorUtility.SetDirty(asset);        
    }
    
}