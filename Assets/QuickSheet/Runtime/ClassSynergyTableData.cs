using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
///
[System.Serializable]
public class ClassSynergyTableData
{
  [SerializeField]
  Clas clas;
  public Clas CLAS { get {return clas; } set { this.clas = value;} }
  
  [SerializeField]
  int[] typecount = new int[0];
  public int[] Typecount { get {return typecount; } set { this.typecount = value;} }
  
  [SerializeField]
  float[] values = new float[0];
  public float[] Values { get {return values; } set { this.values = value;} }
  
}