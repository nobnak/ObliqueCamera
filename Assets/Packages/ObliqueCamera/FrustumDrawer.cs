using UnityEngine;
using System.Collections;
using Gist;
using System.Collections.Generic;
using System.Text;

namespace ObliqueCameraSystem {
	public class FrustumDrawer : System.IDisposable {
		Camera _targetCam;
		GLFigure _fig;

		public FrustumDrawer(Camera targetCam) {
			this._targetCam = targetCam;
			this._fig = new GLFigure ();
		}

		#region IDisposable implementation
		public void Dispose () {
			if (_fig != null) {
				_fig.Dispose ();
				_fig = null;
			}
		}
		#endregion

        public void DrawFrustum(Color color) {
			var ndc2world = _targetCam.cameraToWorldMatrix * _targetCam.projectionMatrix.inverse;
            DrawFrustum (ndc2world, color);
		}

        void DrawFrustum(Matrix4x4 ndc2world, Color color) {
			_fig.DrawLines (FrustumEdges(ndc2world), Camera.current.worldToCameraMatrix, color, GL.LINES);
		}
		IEnumerable<Vector3> FrustumEdges(Matrix4x4 ndc2world) {
			for (var i = 0; i < EdgesInNDC.Length; i += 2) {
				yield return ndc2world.MultiplyPoint3x4 (EdgesInNDC [i]);
				yield return ndc2world.MultiplyPoint3x4 (EdgesInNDC [i + 1]);
			}
		}
		Vector3 NDCWithZ2World(Vector3 pos, Matrix4x4 ndc2world) {
			var f = -_targetCam.farClipPlane;
			var n = -_targetCam.nearClipPlane;
			pos.z = (pos.z - n) / (f - n);
			return ndc2world.MultiplyPoint3x4 (pos);
		}

		#region Static
		public static readonly Vector3[] EdgesInNDC;
		static FrustumDrawer() {
			var list = new List<Vector3> ();
			for (var i = 0; i < 8; i++) {
				foreach (var j in new int[]{ 1, 2, 4 }) {
					var k = i | j;
					if (i != k) {
						list.Add (FromEdgeInt(i));
						list.Add (FromEdgeInt(k));
					}
				}
			}
			EdgesInNDC = list.ToArray ();
		}
		static Vector3 FromEdgeInt(int edge) {
			return new Vector3 (
				(edge & 1) != 0 ? 1f : -1f,
				(edge & (1 << 1)) != 0 ? 1f : -1f,
				(edge & (1 << 2)) != 0 ? 1f : 0f);
		}
		#endregion
	}
}