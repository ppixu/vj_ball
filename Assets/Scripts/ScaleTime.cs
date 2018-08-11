using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScale : MonoBehaviour {

	[SerializeField] float timescale = 1;

	void Start () {
		Time.timeScale = timescale;
	}
	
}
