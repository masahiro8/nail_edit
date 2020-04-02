using UnityEngine;

public class SetCanvasBounds : MonoBehaviour {

	public bool ignoreBottom;
	private RectTransform panel;
	Rect lastSafeArea = new Rect(0, 0, 0, 0);

	// Use this for initialization
	void Start()
	{
		panel = GetComponent<RectTransform>();
	}

	void ApplySafeArea(Rect area)
	{
		var anchorMin = area.position;
		var anchorMax = area.position + area.size;
		anchorMin.x /= Screen.width;
		anchorMin.y /= Screen.height;
		anchorMax.x /= Screen.width;
		anchorMax.y /= Screen.height;
		if (ignoreBottom) {
			anchorMin.y = panel.anchorMin.y;
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
