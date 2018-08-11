using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IncrementTime : MonoBehaviour {

	[SerializeField] TextMeshProUGUI text;
	[SerializeField] string def = "00:0";
	[SerializeField] int seconds = 5;
	private float t = 0;

	void Start () {
		text.SetText(def + seconds);
	}

	void Update () {
		if (t > 1 && seconds > 0) {
			seconds--;
			text.SetText(def + seconds);
			t = 0;
		}

		t = t + Time.deltaTime;
	}
}
