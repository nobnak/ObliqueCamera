using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gist {
	public class GLFigure : System.IDisposable {
        public const string PROP_COLOR = "_Color";
        public const string PROP_SRC_BLEND = "_SrcBlend";
        public const string PROP_DST_BLEND = "_DstBlend";
        public const string PROP_ZWRITE = "_ZWrite";
        public const string PROP_ZTEST = "_ZTest";
        public const string PROP_CULL = "_Cull";
        public const string PROP_ZBIAS = "_ZBias";

		public const float TWO_PI_RAD = 2f * Mathf.PI;
		public const int SEGMENTS = 36;
		public const string LINE_SHADER = "Hidden/Internal-Colored";

        public static readonly Vector3[] QUAD = new Vector3[]{
            new Vector3(-0.5f, -0.5f, 0f), new Vector3(-0.5f,  0.5f, 0f),
            new Vector3( 0.5f,  0.5f, 0f), new Vector3( 0.5f, -0.5f, 0f)            
        };
        
        public enum ZTestEnum { NEVER = 1, LESS = 2, EQUAL = 3, LESSEQUAL = 4,
            GREATER = 5, NOTEQUAL = 6, GREATEREQUAL = 7, ALWAYS = 8 };

		Material _lineMat;

		public GLFigure() {
			var lineShader = Shader.Find (LINE_SHADER);
			if (lineShader == null)
				Debug.LogErrorFormat ("Line Shader not found : {0}", LINE_SHADER);
			_lineMat = new Material (lineShader);
		}

        public bool ZWriteMode {
            get { return _lineMat.GetInt (PROP_ZWRITE) == 1; }
            set { _lineMat.SetInt (PROP_ZWRITE, value ? 1 : 0); }
        }
        public ZTestEnum ZTestMode {
            get { return (ZTestEnum)_lineMat.GetInt (PROP_ZTEST); }
            set { _lineMat.SetInt (PROP_ZTEST, (int)value); }
        }

		public void DrawCircle(Vector3 center, Quaternion look, Vector2 size, Color color) {
			var scale = new Vector3 (size.x, size.y, 1f);
			var modelMat = Matrix4x4.TRS (center, look, scale);
			var cameraMat = Camera.current.worldToCameraMatrix;
			DrawCircle (cameraMat * modelMat, color);
		}
		public void FillCircle(Vector3 center, Quaternion look, Vector2 size, Color color) {
			var scale = new Vector3 (size.x, size.y, 1f);
			var modelMat = Matrix4x4.TRS (center, look, scale);
			var cameraMat = Camera.current.worldToCameraMatrix;
			FillCircle (cameraMat * modelMat, color);
        }
        public void DrawQuad(Vector3 center, Quaternion look, Vector2 size, Color color) {
            var scale = new Vector3 (size.x, size.y, 1f);
            var modelMat = Matrix4x4.TRS (center, look, scale);
            var cameraMat = Camera.current.worldToCameraMatrix;
            DrawQuad (cameraMat * modelMat, color);
        }
        public void FillQuad(Vector3 center, Quaternion look, Vector2 size, Color color) {
            var scale = new Vector3 (size.x, size.y, 1f);
            var modelMat = Matrix4x4.TRS (center, look, scale);
            var cameraMat = Camera.current.worldToCameraMatrix;
            FillQuad (cameraMat * modelMat, color);
        }

		public void DrawCircle (Matrix4x4 modelViewMat, Color color) {
			_lineMat.SetPass (0);
			GL.PushMatrix ();
			GL.LoadIdentity ();
			GL.MultMatrix (modelViewMat);
			var dr = TWO_PI_RAD / SEGMENTS;
			GL.Begin (GL.LINES);
			GL.Color (color);
			var v = new Vector3 (0.5f, 0f, 0f);
			for (var i = 0; i <= SEGMENTS; i++) {
				GL.Vertex (v);
                v.Set(0.5f * Mathf.Cos (i * dr), 0.5f * Mathf.Sin (i * dr), 0f);
				GL.Vertex (v);
			}
			GL.End ();
			GL.PopMatrix ();
		}
		public void FillCircle(Matrix4x4 modelViewMat, Color color) {
			_lineMat.SetPass (0);
			GL.PushMatrix ();
			GL.LoadIdentity ();
			GL.MultMatrix (modelViewMat);
			var dr = TWO_PI_RAD / SEGMENTS;
			GL.Begin (GL.TRIANGLES);
			GL.Color (color);
			var v = new Vector3 (0.5f, 0f, 0f);
			for (var i = 0; i < SEGMENTS; i++) {
				GL.Vertex (v);
				GL.Vertex (Vector3.zero);
                v.Set (0.5f * Mathf.Cos ((i + 1) * dr), 0.5f * Mathf.Sin ((i + 1) * dr), 0f);
				GL.Vertex (v);
			}
			GL.End ();
			GL.PopMatrix ();
		}

        public void DrawQuad(Matrix4x4 modelViewMat, Color color) {
            _lineMat.SetPass (0);
            GL.PushMatrix ();
            GL.LoadIdentity ();
            GL.MultMatrix (modelViewMat);
            GL.Begin (GL.LINES);
            GL.Color (color);
            var v = QUAD [0];
            for (var i = 0; i < QUAD.Length; i++) {
                GL.Vertex (v);
                v = QUAD [(i + 1) % QUAD.Length];
                GL.Vertex (v);
            }
            GL.End ();
            GL.PopMatrix ();
        }
        public void FillQuad(Matrix4x4 modelViewMat, Color color) {
            _lineMat.SetPass (0);
            GL.PushMatrix ();
            GL.LoadIdentity ();
            GL.MultMatrix (modelViewMat);
            GL.Begin (GL.QUADS);
            GL.Color (color);
            for (var i = 0; i < QUAD.Length; i++)
                GL.Vertex (QUAD [i]);
            GL.End ();
            GL.PopMatrix ();
        }
		public void DrawLines(IEnumerable<Vector3> vertices, Transform trs, Color color, int mode) {
			DrawLines (vertices, Camera.current.worldToCameraMatrix * trs.localToWorldMatrix, color, mode);
		}
		public void DrawLines(IEnumerable<Vector3> vertices, Matrix4x4 modelViewMat, Color color, int mode) {
			_lineMat.SetPass (0);
			GL.PushMatrix ();
			GL.LoadIdentity ();
			GL.MultMatrix (modelViewMat);
			GL.Begin (mode);
			GL.Color (color);
			var iter = vertices.GetEnumerator ();
			while (iter.MoveNext ()) {
				var vfrom = iter.Current;
				if (!iter.MoveNext ())
					break;
				var vto = iter.Current;
				GL.Vertex (vfrom);
				GL.Vertex (vto);
			}
			GL.End ();
			GL.PopMatrix ();
		}

		#region IDisposable implementation
		public void Dispose () {
			GameObject.DestroyImmediate(_lineMat);
		}
		#endregion
	}
}