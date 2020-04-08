using UnityEngine;

[CreateAssetMenu(menuName = "Data Tables/MenuTable", fileName = "MenuTable")]
public class MenuTable : ScriptableObject
{
    public MenuRecord[] list;
}
