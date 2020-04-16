using UnityEngine;

[CreateAssetMenu(menuName = "Data Tables/MyListTable", fileName = "MyListTable")]
public class MyListTable : ScriptableObject
{
    public MyListRecord[] list;
}
