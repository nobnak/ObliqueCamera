using UnityEngine;
using System.Collections;
using Gist;

namespace ObliqueCameraSystem {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CrossSection : MonoBehaviour {
        GLFigure _fig;
        Camera _attachedCam;

        void OnEnable() {
            _fig = new GLFigure ();
            _attachedCam = GetComponent<Camera> ();
        }
        void OnDisable() {
            if (_fig != null) {
                _fig.Dispose ();
                _fig = null;
            }
        }
        void OnRenderObject() {
            if (_attachedCam == null || _fig == null)
                return;
            if ((Camera.current.cullingMask & (1 << gameObject.layer)) == 0)
                return;

            
        }
        
    }
}
