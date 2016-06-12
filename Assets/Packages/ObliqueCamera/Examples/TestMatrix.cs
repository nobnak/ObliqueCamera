using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class TestMatrix : MonoBehaviour {
	Camera _attachedCam;

	// Use this for initialization
	void Start () {
		_attachedCam = GetComponent<Camera> ();

		var nearClip = new Vector4 (0f, 0f, -1f, -_attachedCam.nearClipPlane);
		Debug.Log (_attachedCam.projectionMatrix);
		Debug.Log (_attachedCam.CalculateObliqueMatrix (nearClip));
	}
}
