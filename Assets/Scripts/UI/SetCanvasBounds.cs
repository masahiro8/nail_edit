using UnityEngine;

public class SetCanvasBounds : MonoBehaviour {

	public bool ignoreBottom;
	private RectTransform panel;
	Rect lastSafeArea = Rect.zero;

	// Use this for initialization
	void Awake()
	{
		panel = GetComponent<RectTransform>();
		Update(); // 一度更新しておかないとお気に入りリストの高さなどに影響が出る
	}

	void ApplySafeArea(Rect area)
	{
		var anchorMin = area.position;
		var anchorMax = area.position + area.size;
// #if UNITY_EDITOR
// 		// エディタ上でのiPhone Xのシミュレート
// 		if ((float)Screen.height / (float)Screen.width > 2) {
// 			anchorMin.y += Screen.height * 0.05f;
// 			anchorMax.y -= Screen.height * 0.05f;
// 		}
// #endif
		anchorMin.x /= Screen.width;
		anchorMin.y /= Screen.height;
		anchorMax.x /= Screen.width;
		anchorMax.y /= Screen.height;
		if (ignoreBottom) {
			anchorMax.y = panel.anchorMax.y;
		}
		panel.anchorMin = anchorMin;
		panel.anchorMax = anchorMax;

		lastSafeArea = area;
	}

	// Update is called once per frame
	void Update()
	{
		Rect safeArea = Screen.safeArea;

		if (safeArea != lastSafeArea) {
			ApplySafeArea(safeArea);
		}
	}
}
