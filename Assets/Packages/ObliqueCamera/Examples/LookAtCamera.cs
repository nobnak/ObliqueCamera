using UnityEngine;
using System.Collections;

namespace ObliqueCameraSystem {
	[ExecuteInEditMode]
	public class LookAtCamera : MonoBehaviour {
		public Transform target;

		void OnEnable() {
		}
		void Update () {
			transform.LookAt (target, target.up);
		}
	}
}