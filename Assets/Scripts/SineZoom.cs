using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineZoom : MonoBehaviour {

	private float scale;
	private Vector3 defaultScale;
	public float s;
	public float a;
	public float r;

	void Start() {
		defaultScale = this.transform.localScale;
	}

	void Update () {
		scale = r + a * Mathf.Sin(Time.time * s);
		this.transform.localScale = defaultScale * scale;
	}
}
