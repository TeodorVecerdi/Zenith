using UnityEngine;

public class CameraControl : MonoBehaviour {
	public float MaxZoom = 20;
	public float MinZoom = 10;
	public float CurrentZoom;
	public float ZoomSpeed = 2f;
	
	// Use this for initialization
	void Start () {
		CurrentZoom = (MaxZoom + MinZoom) / 2;
		GetComponent<Camera>().orthographicSize = CurrentZoom;
	}
	
	// Update is called once per frame
	void Update () {
		float scrollValue = Input.GetAxis("Mouse ScrollWheel");
		if (scrollValue != 0) {
			CurrentZoom -= scrollValue * ZoomSpeed;
			CurrentZoom = Mathf.Clamp(CurrentZoom, MinZoom, MaxZoom);
			GetComponent<Camera>().orthographicSize = CurrentZoom;
		}
	}
}
