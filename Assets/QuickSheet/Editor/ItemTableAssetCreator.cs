using UnityEngine;
using UnityEditor;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
/// 
public partial class GoogleDataAssetUtility
{
    [MenuItem("Assets/Create/Google/ItemTable")]
    public static void CreateItemTableAssetFile()
    {
        ItemTable asset = CustomAssetUtility.CreateAsset<ItemTable>();
        asset.SheetName = "DataTable";
        asset.WorksheetName = "ItemTable";
        EditorUtility.SetDirty(asset);        
    }
    
}