using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObscureObject : MonoBehaviour {

	void Start() {
		foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
			foreach(Material m in r.materials) {
				m.renderQueue = 2002;
			}
		}
	}

}
