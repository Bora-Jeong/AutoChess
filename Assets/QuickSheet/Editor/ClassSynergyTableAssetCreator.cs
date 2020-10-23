using UnityEngine;
using UnityEditor;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
/// 
public partial class GoogleDataAssetUtility
{
    [MenuItem("Assets/Create/Google/ClassSynergyTable")]
    public static void CreateClassSynergyTableAssetFile()
    {
        ClassSynergyTable asset = CustomAssetUtility.CreateAsset<ClassSynergyTable>();
        asset.SheetName = "DataTable";
        asset.WorksheetName = "ClassSynergyTable";
        EditorUtility.SetDirty(asset);        
    }
    
}