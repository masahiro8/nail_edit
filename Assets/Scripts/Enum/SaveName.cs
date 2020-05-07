using System;
using UnityEngine;

public enum SaveName
{
    TutorialDone, // チュートリアル完了
    MyListItem, // お気に入りリストと持っているリストの保存
    ColorHidden, // カラーごとの非表示
    CategoryHidden, // シリーズごとの非表示
}

public static partial class EnumExtensions
{
    // 取得
    public static bool GetBool(this SaveName type)
    {
        return PlayerPrefs.GetInt(type.ToString(), 0) != 0;
    }

    // 取得
    public static bool GetBool(this SaveName type, string key)
    {
        return PlayerPrefs.GetInt(type.ToString() + key, 0) != 0;
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
    public static void SetBool(this SaveName type, string key, bool value)
    {
        PlayerPrefs.SetInt(type.ToString() + key, value ? 1 : 0);
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

    // 切り替え
    public static void ToggleBool(this SaveName type, string key)
    {
        type.SetBool(key, !type.GetBool(key));
    }
}
