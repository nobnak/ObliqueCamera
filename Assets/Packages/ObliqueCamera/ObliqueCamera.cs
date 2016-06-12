using UnityEngine;
using System.Collections;
using Gist;

namespace ObliqueCameraSystem {
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class ObliqueCamera : MonoBehaviour {
		public Transform target;

		public bool debug;

		Camera _attachedCam;
		FrustumDrawer _drawer;
		Matrix4x4 _proj;
		Matrix4x4 _invProj;

		void OnEnable() {
			_attachedCam = GetComponent<Camera> ();
			_drawer = new FrustumDrawer (_attachedCam);
		}
		void OnDisable() {
			_attachedCam.ResetProjectionMatrix ();
			_drawer.Dispose ();
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

			_proj = Matrix4x4.zero;
			_proj[0] = wInv;	              _proj[8] = -wInv*t.x/t.z;
			                 _proj[5] = hInv; _proj[9] = -hInv*t.y/t.z;
			                                  _proj[10] = fnInv;        _proj[14] = -fnInv*n;
			                                                            _proj [15] = 1f;
			_attachedCam.projectionMatrix = _proj;
			_invProj = _proj.inverse;			
		}
		void OnRenderObject() {
			if (!debug || _drawer == null)
				return;
			if ((Camera.current.cullingMask & (1 << gameObject.layer)) == 0)
				return;
			_drawer.DrawFrustum ();
		}
	}
}