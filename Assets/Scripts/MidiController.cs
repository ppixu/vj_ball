using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MidiJack;

public class MidiController : MonoBehaviour {

	public AnimationCurve curveSmooth;
	public AnimationCurve curveInverse;
	public AnimationCurve curveIn;
	public AnimationCurve curveOut;
	public Renderer ball;
	public Renderer[] ground;
	public SkinnedMeshRenderer[] landMasks;
	public Image BG;

	// AudioViz
    public GameObject[] Objects;
    public float Scale = 10f;
    private float[] Samples = new float[1024];

	public int sample = 0;
    FFTWindow fftWindow;
    AudioSource thisAudioSource;

    void Start()
    {
        thisAudioSource = GetComponent<AudioSource>();
        fftWindow = FFTWindow.Triangle;
    }

	private float ballScale = 2;
	private float vizScale = 0;
	private IEnumerator BallScale;

	private float ballHeight = 1;
	private float vizHeight = 0;
	private float ballFresnel = 1;
	private float vizFresnel = 0;
	void Update () {
        thisAudioSource.GetSpectrumData(Samples, 0, fftWindow);

		// Knobs
		// 1
		if (MidiInput.GetPad(1) <= 0) {
			if (MidiInput.GetKnob(1) >= 0) 
		        ballHeight = 5 + -10 * curveInverse.Evaluate(MidiInput.GetKnob(1));
		} else { 
			if (MidiInput.GetKnob(1) >= 0) 
				vizHeight = Samples[sample] * curveSmooth.Evaluate(MidiInput.GetKnob(1)) * 20;
		}
		ball.material.SetFloat("_Height", ballHeight + vizHeight);



   		if (MidiInput.GetKnob(2) >= 0) 
		    StartCoroutine(BallLerpValue("_Speed", 		curveSmooth.Evaluate(MidiInput.GetKnob(2))));

		// 2
		if (MidiInput.GetKnob(26) <= 0) {
			if (MidiInput.GetKnob(3) >= 0) 
		        ballFresnel = 100 * curveIn.Evaluate(MidiInput.GetKnob(3));
		} else { 
			if (MidiInput.GetKnob(3) >= 0) 
				vizFresnel = Samples[sample] * -100 * curveIn.Evaluate(MidiInput.GetKnob(3));
		}
		ball.material.SetFloat("_Fresnel", ballFresnel + vizFresnel);


   		if (MidiInput.GetKnob(4) >= 0) 
			StartCoroutine(BallLerpValue("_Scale", 		100 * MidiInput.GetKnob(4)));

		// 5

		if (MidiInput.GetKnob(20) <= 0) {
			if (MidiInput.GetKnob(0) >= 0) {
				ballScale = 0.01f + 30 *						curveSmooth.Evaluate(MidiInput.GetKnob(0));
				StartCoroutine(BallLerpScale());
				// ball.gameObject.transform.localScale = new Vector3(ballScale, ballScale, ballScale);
			}
		} else {
			if (MidiInput.GetKnob(0) >= 0) {
				vizScale = Samples[sample] * /*curveSmooth.Evaluate(MidiInput.GetKnob(5))*/ 20;
				float s = ballScale + vizScale;
				ball.gameObject.transform.localScale = new Vector3(s,s,s);
				// ball.gameObject.transform.localScale = new Vector3(vizScale, vizScale, vizScale);
			} 
		}

		if (MidiInput.GetKnob(6) >= 0) 
			BG.color = new Color(255,255,255, 	curveSmooth.Evaluate(MidiInput.GetKnob(6)));
		if (MidiInput.GetKnob(7) >= 0) {
			foreach (Renderer r in ground) {
    	    	r.material.SetFloat("_Amount", curveSmooth.Evaluate(MidiInput.GetKnob(7)));
			}
		}
		if (MidiInput.GetKnob(8) >= 0) {
			foreach (Renderer r in ground) {
		       r.material.SetFloat("_Speed", 	curveSmooth.Evaluate(MidiInput.GetKnob(8)));
			}
		}

		if (MidiInput.GetKnob(9) >= 0) {
			Debug.Log(MidiInput.GetKnob(9));
			foreach (SkinnedMeshRenderer r in landMasks) {
				r.SetBlendShapeWeight(0, 2.84f + 2.2f * curveSmooth.Evaluate(MidiInput.GetKnob(9)));
			}
		}
		// Pads
		if (MidiInput.GetPad(51) > 0) {}

		// AudioViz

	}
	
	IEnumerator BallLerpScale () {
		Vector3 begin = ball.gameObject.transform.localScale;
		Vector3 end = new Vector3(ballScale,ballScale,ballScale);
		float t = 0;
		while (t < 0.05f) {
			ball.gameObject.transform.localScale = Vector3.Lerp(begin, end, curveOut.Evaluate(t/0.05f));
			t = t + Time.deltaTime;
			yield return null;
		}
		yield return null;
	}


	IEnumerator BallLerpValue (string val, float end) {
		float begin = ball.material.GetFloat(val);
		float t = 0;
		while (t < 0.05f) {
			ball.material.SetFloat(val, Mathf.Lerp(begin, end, t/0.05f));
			t = t + Time.deltaTime;
			yield return null;
		}
		yield return null;
	}

	float map(float x, float in_min, float in_max, float out_min, float out_max)
	{
		return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
	}
}
