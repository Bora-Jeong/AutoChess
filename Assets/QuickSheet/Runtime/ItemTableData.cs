using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
///
[System.Serializable]
public class ItemTableData
{
  [SerializeField]
  int typeid;
  public int Typeid { get {return typeid; } set { this.typeid = value;} }
  
  [SerializeField]
  string name;
  public string Name { get {return name; } set { this.name = value;} }
  
  [SerializeField]
  int gold;
  public int Gold { get {return gold; } set { this.gold = value;} }
  
  [SerializeField]
  UseType usetype;
  public UseType USETYPE { get {return usetype; } set { this.usetype = value;} }
  
  [SerializeField]
  float value;
  public float Value { get {return value; } set { this.value = value;} }
  
}