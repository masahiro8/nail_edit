using UnityEngine;

[CreateAssetMenu(menuName = "Data Tables/NailTable", fileName = "NailTable")]
public class NailTable : ScriptableObject
{
    public NailRecord[] list;
}
