using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices; // 관리되지않는 코드.

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class HelloPacket : HNET.Packet
{
    public HelloPacket() : base(999)
    {
        a[0] = 7;
        a[1] = 8;
    }

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public int[] a = new int[2];

    [MarshalAs(UnmanagedType.I1)]
    public bool b = true;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string c = "hello";
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class WorldPacket : HNET.Packet
{
    public WorldPacket() : base(777) { }

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public int[] a = new int[2];

    [MarshalAs(UnmanagedType.I1)]
    public bool b;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string c;
}