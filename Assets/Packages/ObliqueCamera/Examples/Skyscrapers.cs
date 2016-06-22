using UnityEngine;
using System.Collections;

namespace ObliqueCameraSystem {
    public class Skyscrapers : MonoBehaviour {
        public GameObject buildingfab;
        public float radius = 5f;
        public float size = 2f;
        public float sizeVariation = 0.2f;
        public int count = 100;

        GameObject[] _buildings;

    	// Use this for initialization
    	void Start () {
            _buildings = new GameObject[count];
            for (var i = 0; i < count; i++) {
                var b = _buildings[i] = Instantiate (buildingfab);
                var tr = b.transform;
                var sampleInCircle = Random.insideUnitCircle;
                tr.SetParent (transform, false);
                tr.localPosition = radius * new Vector3 (sampleInCircle.x, 0f, sampleInCircle.y);
                tr.localRotation = Quaternion.Euler (0f, Random.Range (0f, 360f), 0f);
                tr.localScale = (1f + sizeVariation * Random.value) * size * Vector3.one;
            }
    	}
    }
}