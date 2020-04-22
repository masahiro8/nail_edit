using UnityEngine;
using Unity.Mathematics;

public static class StringExtension
{
    public static string Localized(this string str)
    {
        return DataTable.Localized.localization.GetLocalizedString(str);
    }
}
