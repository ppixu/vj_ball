using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineMovement : MonoBehaviour {

	[SerializeField]
	Vector3[] axis = {new Vector3 (0,1,0)};

	[SerializeField]
	float[] speed = {1f};	

	[SerializeField]
	float[] magnitude = {1f};	

	Vector3 defaultPos;
	// Use this for initialization
	void Start () {
		defaultPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = this.defaultPos;
		for (int i = 0; i < axis.Length ; i++) {
			this.transform.position += axis[i] * Mathf.Sin(speed[i] * Time.time) * magnitude[i];
		}
	}
}
