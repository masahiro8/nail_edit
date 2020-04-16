using UnityEngine;
using UniRx;
// using SubjectNerd.Utilities;

[CreateAssetMenu(menuName = "Data Tables/TutorialTable", fileName = "TutorialTable")]
public class TutorialTable : ScriptableObject
{
    // [Reorderable]
    public TutorialRecord[] list;
}
