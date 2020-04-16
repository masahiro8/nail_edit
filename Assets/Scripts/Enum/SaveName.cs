using System;
using UnityEngine;

public enum SaveName
{
    TutorialDone,
}

public static partial class EnumExtensions
{
    // 取得
    public static bool GetBool(this SaveName type)
    {
        return PlayerPrefs.GetInt(type.ToString(), 0) != 0;
    }

    // 取得
    public static int GetInt(this SaveName type)
    {
        return PlayerPrefs.GetInt(type.ToString(), 0);
    }

    // 取得
    public static string GetString(this SaveName type)
    {
        return PlayerPrefs.GetString(type.ToString(), "");
    }

    // 保存
    public static void SetBool(this SaveName type, bool value)
    {
        PlayerPrefs.SetInt(type.ToString(), value ? 1 : 0);
        PlayerPrefs.Save();
    }

    // 保存
    public static void SetInt(this SaveName type, int value)
    {
        PlayerPrefs.SetInt(type.ToString(), value);
        PlayerPrefs.Save();
    }

    // 保存
    public static void SetString(this SaveName type, string value)
    {
        PlayerPrefs.SetString(type.ToString(), value);
        PlayerPrefs.Save();
    }
}
