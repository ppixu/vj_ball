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

	private float[] hiddenValues = new float[17];
	
	[Range(0f, 1f)] public float[] knobs;
	public bool[] pads = new bool[17];

	private float soundScale = 0;
	private float ballScale = 2;
	private float ballRippleScale = 2;
	private float ballHeight = 1;
	private float ballFresnel = 1;
	private float ballSpeed = 0;

	private float maskCover = 0;

    void Start()
    {
        thisAudioSource = GetComponent<AudioSource>();
        fftWindow = FFTWindow.Triangle;

		for (int i = 0; i < 17; i++) {
			hiddenValues[i] = knobs[i];
		}

    }

	void Update () {
		// Map midi to sliders
		if(MidiInput.Receiving) 
			getRelativeMidi();

		// Map sliders to variables
		ballScale = 0.2f + 30 * curveSmooth.Evaluate(knobs[0]);
		ballHeight = 3 - 6 * curveInverse.Evaluate(knobs[1]);
   		ballSpeed = curveSmooth.Evaluate(knobs[2]);
		ballFresnel = 100 * curveIn.Evaluate(knobs[3]);
   		ballRippleScale = 100 * curveSmooth.Evaluate(knobs[4]);
		BG.color = new Color(255,255,255, curveSmooth.Evaluate(knobs[5]));
		foreach (Renderer r in ground) r.material.SetFloat("_Amount", curveSmooth.Evaluate(knobs[6]));
		foreach (Renderer r in ground) r.material.SetFloat("_Speed", curveSmooth.Evaluate(knobs[7]));
		maskCover = 2.84f + 2.2f * curveSmooth.Evaluate(knobs[8]);
		// slider 10
		// slider 11
		// slider 12
		// slider 13
		// slider 14
		// slider 15
		// slider 16


		// Sound react
        thisAudioSource.GetSpectrumData(Samples, 0, fftWindow);

		soundScale = Samples[sample] * (pads[1] ? 1 : 0);
		// toggle2
		// toggle3
		// toggle4
		// toggle5
		// toggle6
		// toggle7
		// toggle8
		// toggle9
		// toggle10
		// toggle11
		// toggle12
		// toggle13
		// toggle14
		// toggle15
		// toggle16

		// Set variables and account for sliders and soundScale
		ball.material.SetFloat("_Height", ballHeight + soundScale);
		ball.material.SetFloat("_Fresnel", ballFresnel /* + soundScale */);
		ball.material.SetFloat("_Speed", ballSpeed /* + soundScale */);
		ball.material.SetFloat("_Scale", ballRippleScale /* + soundScale */);
		foreach (SkinnedMeshRenderer r in landMasks) r.SetBlendShapeWeight(0, maskCover + soundScale);
		ball.transform.localScale = new Vector3(ballScale, ballScale, ballScale);
	}

	private void getMidi() {

		for (int i = 0; i < 17; i++) {
			if (MidiInput.GetKnob(i) >= 0) knobs[i] = MidiInput.GetKnob(i);
		}
		for (int i = 0; i < 17; i++) {
			if (MidiInput.GetPad(i) == 1) { pads[i] = true; } else { pads[i] = false; }
		}

	}	

	private void getRelativeMidi() {
		for (int i = 0; i < 17; i++) {
			if (MidiInput.GetRelativeKnob(i) > -1) {
				hiddenValues[i] += MidiInput.GetRelativeKnob(i);
				knobs[i] = 1f + (-0.5f * (1f + Mathf.Cos(Mathf.PI * hiddenValues[i])));
				// slider[i] = Mathf.PingPong(hiddenValues[i], 1);
			}
		}
		for (int i = 0; i < 17; i++) {
			if (MidiInput.GetPad(i) == 1) { pads[i] = true; } else { pads[i] = false; }
		}
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

	float map(float x, float out_min, float out_max)
	{
		return (x - 0) * (out_max - out_min) / (1 - 0) + out_min;
	}
}
