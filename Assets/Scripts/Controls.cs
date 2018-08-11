using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {

	[Range(0f, 1f)] public float[] knobs;
	public bool[] pads = new bool[17];
	public bool[] knobChanged = new bool[17];
	public bool[] padsChanged = new bool[17];

}
