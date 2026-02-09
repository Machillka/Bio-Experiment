using System;
using UnityEngine;

public enum NucleoBaseTypes
{
    A, T, C, G, Unknown
}

// 纯数据类
public class NucleoData
{
    public NucleoBaseTypes type;
    public NucleoData(NucleoBaseTypes t) => type = t;
    public override string ToString()
    {
        return type.ToString();
    }

    public Color GetColor()
    {
        return type switch
        {
            NucleoBaseTypes.A => Color.green,
            NucleoBaseTypes.T => Color.red,
            NucleoBaseTypes.C => Color.blue,
            NucleoBaseTypes.G => Color.yellow,
            _ => Color.white
        };
    }

    public NucleoBaseTypes GetComplement()
    {
        return type switch
        {
            NucleoBaseTypes.A => NucleoBaseTypes.T,
            NucleoBaseTypes.T => NucleoBaseTypes.A,
            NucleoBaseTypes.C => NucleoBaseTypes.G,
            NucleoBaseTypes.G => NucleoBaseTypes.C,
            _ => NucleoBaseTypes.Unknown
        };
    }

    /// <summary>
    /// 获得碱基互补配对的类型
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    static public NucleoBaseTypes GetComplement(NucleoBaseTypes n)
    {
        return n switch
        {
            NucleoBaseTypes.A => NucleoBaseTypes.T,
            NucleoBaseTypes.T => NucleoBaseTypes.A,
            NucleoBaseTypes.C => NucleoBaseTypes.G,
            NucleoBaseTypes.G => NucleoBaseTypes.C,
            _ => NucleoBaseTypes.Unknown
        };
    }
}