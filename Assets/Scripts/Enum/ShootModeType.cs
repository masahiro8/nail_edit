using System;
using System.Linq;
using UnityEngine;

public enum ShootModeType
{
    Free,
    HandPaa,
    HandGoo,
    Foot,
}

public static partial class EnumExtensions
{
    public static string GetName(this ShootModeType type, int n)
    {
        string[] names = { "fd", "fm", "gd", "gm", "pd", "pm" };
        return names[n];
    }

    // 取得
    public static int GetIndex(this ShootModeType type, float[,] data)
    {
        if (SROptions.Current.UseAllGooDeco) {
            return 3;
        }
        // int[,] filter = {
        //     { 0, 1, 2, 3, 4, 5 },
        //     { 4, 5 },
        //     { 2, 3 },
        //     { 0, 1 },
        // };
        var n1 = 0;
        var n2 = 1;
        switch (type) {
            case ShootModeType.HandPaa:
                n1 = 4;
                n2 = 5;
                break;
            case ShootModeType.HandGoo:
                n1 = 2;
                n2 = 3;
                if (SROptions.Current.NoUseGooDeco) {
                    n1 = n2;
                }
                break;
            case ShootModeType.Foot:
                n1 = 0;
                n2 = 1;
                break;
            case ShootModeType.Free:
            default:
                var n = data
                    .Cast<float>()
                    .Select((v, index) => new {Index = index, Value = v})
                    .OrderByDescending(v => v.Value)
                    .ToArray()[0].Index;
                if (SROptions.Current.NoUseGooDeco && n == 2) {
                    n = 3;
                }
                return n;
        }

        return data[0, n1] >= data[0, n2] ? n1 : n2;
    }

    // 取得
    public static bool IsUseFreeModeAlert(this ShootModeType type)
    {
        switch (type) {
            case ShootModeType.Free:
                return true;
            default:
                return false;
        }
    }

    // // 取得
    // public static void SetWithAlert(this ShootModeType src, ShootModeType dst)
    // {
    // }
}
