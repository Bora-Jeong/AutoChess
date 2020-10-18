using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum Type
    {
        Inventory,
        MyField,
        EnemyField
    }

    [SerializeField]
    private Type _type;

    public Type type => _type;
    public bool isOccupied { get; set; }
}
