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
			hiddenValues[i] = c.knobs[i];
		}

    }

	void Update () {
		// Map midi to sliders
		if(MidiInput.Receiving) 
			getRelativeMidi();

		// Map sliders to variables
		ballScale = 0.2f + 30 * curveSmooth.Evaluate(c.knobs[0]);
		ballHeight = 3 - 6 * curveInverse.Evaluate(c.knobs[1]);
   		ballSpeed = curveSmooth.Evaluate(c.knobs[2]);
		ballFresnel = 100 * curveIn.Evaluate(c.knobs[3]);
   		ballRippleScale = 100 * curveSmooth.Evaluate(c.knobs[4]);
		BG.color = new Color(255,255,255, curveSmooth.Evaluate(c.knobs[5]));
		foreach (Renderer r in ground) r.material.SetFloat("_Amount", curveSmooth.Evaluate(c.knobs[6]));
		foreach (Renderer r in ground) r.material.SetFloat("_Speed", curveSmooth.Evaluate(c.knobs[7]));
		maskCover = 2.84f + 2.2f * curveSmooth.Evaluate(c.knobs[8]);
		// knob 10
		// knob 11
		// knob 12
		// knob 13
		// knob 14
		// knob 15
		// knob 16


		// Sound react
        thisAudioSource.GetSpectrumData(Samples, 0, fftWindow);

		soundScale = Samples[sample] * (c.pads[1] ? 1 : 0);
		// pad2
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
		// pad15
		// pad16

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
			if (MidiInput.GetKnob(i) >= 0) c.knobs[i] = MidiInput.GetKnob(i);
		}
		for (int i = 0; i < 17; i++) {
			if (MidiInput.GetPad(i) == 1) { c.pads[i] = true; } else { c.pads[i] = false; }
		}

	}	

	private void getRelativeMidi() {
		for (int i = 0; i < 17; i++) {
			if (MidiInput.GetRelativeKnob(i) > -1) {
				hiddenValues[i] += MidiInput.GetRelativeKnob(i);
				c.knobs[i] = 1f + (-0.5f * (1f + Mathf.Cos(Mathf.PI * hiddenValues[i])));
				// slider[i] = Mathf.PingPong(hiddenValues[i], 1);
			}
		}
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
