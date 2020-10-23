using UnityEngine;
using UnityEditor;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
/// 
public partial class GoogleDataAssetUtility
{
    [MenuItem("Assets/Create/Google/SpeciesSynergyTable")]
    public static void CreateSpeciesSynergyTableAssetFile()
    {
        SpeciesSynergyTable asset = CustomAssetUtility.CreateAsset<SpeciesSynergyTable>();
        asset.SheetName = "DataTable";
        asset.WorksheetName = "SpeciesSynergyTable";
        EditorUtility.SetDirty(asset);        
    }
    
}