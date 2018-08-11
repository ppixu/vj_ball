using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LookAt : MonoBehaviour {

	[SerializeField] Transform target;

	void Update() {
			this.transform.LookAt(target, this.transform.up);
	}

}
