using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MidiJack;

public class MidiController : MonoBehaviour {

	public Controls c;
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

	public GameObject PinkBG;
	public GameObject CloudBall;
	public GameObject CloudBG;
	private float soundScale = 0;
	private float ballScale = 2;
	private float ballRippleScale = 2;
	private float ballHeight = 1;
	private float ballFresnel = 1;
	private float ballSpeed = 0;
	private float maskCover = 0;

	private float ballScaleVelocity = 0;
	private float ballRippleScaleVelocity = 0;
	private float ballHeightVelocity = 0;
	private float ballFresnelVelocity = 0;
	private float ballSpeedVelocity = 0;
	private float maskCoverVelocity = 0;

	private float soundScaleVelocity = 0;
    void Start()
    {
        thisAudioSource = GetComponent<AudioSource>();
        fftWindow = FFTWindow.Triangle;

		for (int i = 0; i < 17; i++) {
			hiddenValues[i] = c.knobs[i];
		}

    }

	void Update () {
		// Map midi to sliders
		if(MidiInput.Receiving) 
			getRelativeMidi();


		// Store previous

		// Map sliders to variables

		ballScale = c.knobChanged[0] ? Mathf.SmoothDamp(ballScale, 0.2f + 30 * curveSmooth.Evaluate(c.knobs[0]), ref ballScaleVelocity, .05f) : ballScale;
		ballHeight = Mathf.SmoothDamp(ballHeight, 3 - 6 * curveInverse.Evaluate(c.knobs[1]), ref ballHeightVelocity, 0.05f);
   		ballSpeed = Mathf.SmoothDamp(ballSpeed, curveSmooth.Evaluate(c.knobs[2]), ref ballSpeedVelocity, .05f);
		ballFresnel = Mathf.SmoothDamp(ballFresnel, 100 * curveIn.Evaluate(c.knobs[3]), ref ballFresnelVelocity, .05f);
   		ballRippleScale = Mathf.SmoothDamp(ballRippleScale, 100 * curveSmooth.Evaluate(c.knobs[4]), ref ballRippleScaleVelocity, .05f);
		BG.color = new Color(255,255,255, curveSmooth.Evaluate(c.knobs[5]));
		foreach (Renderer r in ground) r.material.SetFloat("_Amount", curveSmooth.Evaluate(c.knobs[6]));
		foreach (Renderer r in ground) r.material.SetFloat("_Speed", curveSmooth.Evaluate(c.knobs[7]));
		maskCover = Mathf.SmoothDamp(maskCover, 2.84f + 2.2f * curveSmooth.Evaluate(c.knobs[8]), ref maskCoverVelocity, .05f);
		// knob 10
		// knob 11
		// knob 12
		// knob 13
		// knob 14
		// knob 15
		// knob 16


		// Sound react
        thisAudioSource.GetSpectrumData(Samples, 0, fftWindow);

		soundScale = Mathf.SmoothDamp(soundScale, Samples[sample] * (c.pads[1] ? 2 : 0), ref soundScaleVelocity, .01f);
		CloudBall.SetActive(c.pads[2] ? true : false);
		// pad3
		// pad4
		// pad5
		// pad6
		// pad7
		// pad8
		// pad9
		// pad10
		// pad11
		// pad12
		// pad13
		// pad14
		CloudBG.SetActive(c.pads[15] ? true : false);
		PinkBG.SetActive(c.pads[16] ? true : false);



		// Set variables and account for sliders and soundScale
		if (ballHeightVelocity != 0) ball.material.SetFloat("_Height", ballHeight + soundScale);
		if (ballFresnelVelocity != 0) ball.material.SetFloat("_Fresnel", ballFresnel /* + soundScale */);
		if (ballSpeedVelocity != 0) ball.material.SetFloat("_Speed", ballSpeed /* + soundScale */);
		if (ballRippleScaleVelocity != 0) ball.material.SetFloat("_Scale", ballRippleScale /* + soundScale */);
		foreach (SkinnedMeshRenderer r in landMasks) r.SetBlendShapeWeight(0, maskCover + soundScale);
		if (ballScaleVelocity != 0) ball.transform.localScale = new Vector3(ballScale, ballScale, ballScale);
	}

	private void getMidi() {

		for (int i = 0; i < 17; i++) {
			if (MidiInput.GetKnob(i) >= 0) c.knobs[i] = MidiInput.GetKnob(i);
		}
		for (int i = 0; i < 17; i++) {
			if (MidiInput.GetPad(i) == 1) { c.pads[i] = true; } else { c.pads[i] = false; }
		}

	}	

	private void getRelativeMidi() {
		int k = MidiInput.LatestKnob;
		hiddenValues[k] += MidiInput.LatestValue;
		// c.knobs[k] = 1f + (-0.5f * (1f + Mathf.Cos(Mathf.PI * hiddenValues[k])));
		c.knobs[k] = Mathf.PingPong(hiddenValues[k], 1);
		c.knobChanged[k] = true;
		for (int i = 0; i < 17; i++) {
			if (MidiInput.GetPad(i) == 1) { c.pads[i] = true; } else { c.pads[i] = false; }
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
