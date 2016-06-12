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
		GLFigure _fig;
		Matrix4x4 _proj;
		Matrix4x4 _invProj;

		void OnEnable() {
			_attachedCam = GetComponent<Camera> ();
			_fig = new GLFigure ();
		}
		void OnDisable() {
			_attachedCam.ResetProjectionMatrix ();
			_fig.Dispose ();
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
			if (!debug || target == null || _attachedCam == null)
				return;

			if ((Camera.current.cullingMask & (1 << gameObject.layer)) == 0)
				return;
			var t = _attachedCam.worldToCameraMatrix.MultiplyPoint3x4 (target.position);
			var linesInViewport = new Vector3[] {
				new Vector3 (-1f, -1f, t.z), new Vector3 (-1f,  1f, t.z),
				new Vector3 (-1f,  1f, t.z), new Vector3 ( 1f,  1f, t.z),
				new Vector3 ( 1f,  1f, t.z), new Vector3 ( 1f, -1f, t.z),
				new Vector3 ( 1f, -1f, t.z), new Vector3 (-1f, -1f, t.z)
			};
			var linesInWorld = new Vector3[linesInViewport.Length];
			for (var i = 0; i < linesInViewport.Length; i++)
				linesInWorld [i] = NDCWithZ2World(linesInViewport [i]);
			_fig.DrawLines (linesInWorld, Camera.current.worldToCameraMatrix, Color.white, GL.LINES);
		}
		Vector3 NDCWithZ2World(Vector3 pos) {
			var f = -_attachedCam.farClipPlane;
			var n = -_attachedCam.nearClipPlane;
			pos.z = (pos.z - n) / (f - n);
			return (_attachedCam.cameraToWorldMatrix * _invProj).MultiplyPoint3x4 (pos);
		}
	}
}