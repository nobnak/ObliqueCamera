using UnityEngine;
using System.Collections;

namespace ObliqueCameraSystem {
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class ObliqueCamera : MonoBehaviour {
		public Transform target;

		Camera _attachedCam;

		void OnEnable() {
			_attachedCam = GetComponent<Camera> ();
		}
		void OnDisable() {
			_attachedCam.ResetProjectionMatrix ();
		}
		void Update () {
			if (target == null)
				return;

			var t = _attachedCam.worldToCameraMatrix.MultiplyPoint3x4(target.position);
			if (-Mathf.Epsilon <= t.z && t.z <= Mathf.Epsilon)
				return;

			var h = _attachedCam.orthographicSize;
			var w = _attachedCam.aspect * h;
			var n = -_attachedCam.nearClipPlane;
			var f = -_attachedCam.farClipPlane;

			var hInv = 1f / h;
			var wInv = 1f / w;
			var fn = f - n;
			var fnInv = 1f / fn;

			var proj = Matrix4x4.zero;
			proj[0] = wInv;	                         proj[8] = -wInv*t.x/t.z;
			                proj[5] = hInv;          proj[9] = -hInv*t.y/t.z;
			                                    	 proj[10] = fnInv;        proj[14] = -fnInv*n;
			                                                                  proj [15] = 1f;
			_attachedCam.projectionMatrix = proj;
		}
	}
}