using System;
using UnityEngine;

public enum SaveName
{
    TutorialDone, // チュートリアル完了
    NavigationDone, // ナビゲーション完了
    NavigationGuideDone, // ナビゲーション完了
    FreeModeAlertDone, // フリーモードのアラートを表示しない
    MyListItem, // お気に入りリストと持っているリストの保存
    ColorHidden, // カラーごとの非表示
    CategoryHidden, // シリーズごとの非表示
    StoreSelect, //ストア選択
    NailItemFilter, // 絞り込みフィルター
    NailItemTopcoat, // トップコートの状態
    ShootHand, // 撮影の手の方向
    ShootMode, // 撮影モード

    // API
    AppID, // app_id
    AppToken, // app_token

    // デバッグ用
    NailMeshReverse, // ネイルのメッシュを反転
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
    public static int GetInt(this SaveName type, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(type.ToString(), defaultValue);
    }

    // 取得
    public static string GetString(this SaveName type, string defaultValue = null)
    {
        return PlayerPrefs.GetString(type.ToString(), defaultValue);
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
    public static void ToggleBool(this SaveName type)
    {
        type.SetBool(!type.GetBool());
    }

    // 切り替え
    public static void ToggleBool(this SaveName type, string key)
    {
        type.SetBool(key, !type.GetBool(key));
    }
}
