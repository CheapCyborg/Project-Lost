using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraRaycaster))]
public class CursorAffordance : MonoBehaviour {

	[SerializeField] Texture2D walkCursor = null;
	[SerializeField] Texture2D enemyCursor = null;
	[SerializeField] Texture2D unkownCursor = null;
	[SerializeField] Vector2 cursorHotspot = new Vector2(96, 96);

	CameraRaycaster cameraRaycaster;
	
	// Use this for initialization
	void Start () {
		cameraRaycaster = GetComponent<CameraRaycaster>();
		cameraRaycaster.layerChangeObservers += OnLayerChange;
	}

	void OnLayerChange (Layer newLayer) {
		switch (newLayer) {
			case Layer.Walkable:
				Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
				break;
			case Layer.Enemy:
				Cursor.SetCursor(enemyCursor, cursorHotspot, CursorMode.Auto);
				break;
			case Layer.RaycastEndStop:
				Cursor.SetCursor(unkownCursor, cursorHotspot, CursorMode.Auto);
				break;
			default:
				Debug.LogError("DONT KNOW WHAT CURSOR TO SHOW!");
				return;
		}
	}
}
