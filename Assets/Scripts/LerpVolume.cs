using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpVolume : MonoBehaviour {

	private AudioSource source;
	private float volume;

	void Start () {
		source = FindObjectOfType<AudioSource>();
		volume = source.volume;
		StartCoroutine(fadeDown());
	}

	private IEnumerator fadeDown() {
		float t = 0;
		while (t < 0.4f) {
			source.volume = volume - t;
			t = t + Time.deltaTime;
			yield return null;
		}
		yield return null;
	}
}
