using UnityEngine;
using UnityEditor;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
/// 
public partial class GoogleDataAssetUtility
{
    [MenuItem("Assets/Create/Google/StringTable")]
    public static void CreateStringTableAssetFile()
    {
        StringTable asset = CustomAssetUtility.CreateAsset<StringTable>();
        asset.SheetName = "DataTable";
        asset.WorksheetName = "StringTable";
        EditorUtility.SetDirty(asset);        
    }
    
}