using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {

	[Range(0f, 1f)] public float[] knobs = new float[9];
	public bool[] pads = new bool[9];
	public bool[] knobChanged = new bool[9];
	public bool[] padsChanged = new bool[9];

}
