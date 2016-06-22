using UnityEngine;
using System.Collections;

namespace ObliqueCameraSystem {
    [RequireComponent(typeof(Collider))]
    public class TestObliqueCamera : MonoBehaviour {
        public float scale = 0.1f;
        public Camera targetCam;

        Collider _collider;

        void Start() {
            _collider = GetComponent<Collider> ();
        }
    	void Update () {
            if (Input.GetMouseButton (0)) {
                var posScreen = Input.mousePosition;
                var posCam = targetCam.transform.position;
                posCam.x = scale * (posScreen.x - 0.5f * Screen.width);
                posCam.z = scale * (posScreen.y - 0.5f * Screen.height);
                targetCam.transform.position = posCam;
            }
    	}
    }
}