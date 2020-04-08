using UnityEngine;
using UniRx;

[CreateAssetMenu(menuName = "Data Tables/CategoryTable", fileName = "CategoryTable")]
public class CategoryTable : ScriptableObject
{
    public CategoryRecord[] list;
}
