using UnityEngine;
using UnityEditor;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
/// 
public partial class GoogleDataAssetUtility
{
    [MenuItem("Assets/Create/Google/UnitTable")]
    public static void CreateUnitTableAssetFile()
    {
        UnitTable asset = CustomAssetUtility.CreateAsset<UnitTable>();
        asset.SheetName = "DataTable";
        asset.WorksheetName = "UnitTable";
        EditorUtility.SetDirty(asset);        
    }
    
}