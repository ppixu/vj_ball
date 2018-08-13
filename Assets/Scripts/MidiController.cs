using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using MidiJack;


public class MidiController : MonoBehaviour {

	public Controls c;
	public AnimationCurve curveSmooth;
	public AnimationCurve curveInverse;
	public AnimationCurve curveIn;
	public AnimationCurve curveOut;
	public AnimationCurve curveDolly;
	public GameObject models;
	public Renderer ball;
	public Renderer[] ground;
	public SkinnedMeshRenderer[] landMasks;
	public SpriteRenderer BG;

	// AudioViz
    public GameObject[] Objects;
    public float Scale = 10f;
    private float[] Samples = new float[1024];

	public int sample = 0;
    FFTWindow fftWindow;
    AudioSource thisAudioSource;

	public Camera[] cameras;
	public GameObject cameraRoot;

	private float[] hiddenValues = new float[17];

	public GameObject PinkBG;
	public GameObject CloudBall;
	public GameObject ShutterBall;
	public GameObject CloudBG;

	public PostProcessVolume bloomVolume;
	public PostProcessVolume dofVolume;
	private float soundScale = 0;
	private float ballScale = 2;
	private float ballRippleScale = 2;
	private float ballHeight = 1;
	private float ballFresnel = 1;
	private float ballSpeed = 0;
	private float maskCover = 0;
	private float hitchCockZoom = 0;
	private float hitchCockDist = 0;

	private float landMoveAmount;
	private float landMoveSpeed;

	private float bgColor;
	private float bgColorVelocity;

	private float landMoveAmountVelocity;
	private float landMoveSpeedVelocity;
	private float hitchCockVelocity = 0;
	private float ballScaleVelocity = 0;
	private float ballRippleScaleVelocity = 0;
	private float ballHeightVelocity = 0;
	private float ballFresnelVelocity = 0;
	private float ballSpeedVelocity = 0;
	private float maskCoverVelocity = 0;

	private float bloomVolumeFade = 0;
	private float bloomVolumeFadeVelocity = 0;

	private float dofVolumeFade = 0;
	private float dofVolumeFadeVelocity = 0;
	private float soundScaleVelocity = 0;



	// Smoothdamp amount
	private float lag = .05f;
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
			getMidi();


		// Store previous

		// Map sliders to variables
   		ballRippleScale = Mathf.SmoothDamp(ballRippleScale, 100 * curveSmooth.Evaluate(c.knobs[0]), ref ballRippleScaleVelocity, lag);
		ballHeight = Mathf.SmoothDamp(ballHeight, 3 - 6 * curveInverse.Evaluate(c.knobs[1]), ref ballHeightVelocity, lag);
		ballFresnel = Mathf.SmoothDamp(ballFresnel, 100 * curveIn.Evaluate(c.knobs[2]), ref ballFresnelVelocity, lag);
		landMoveAmount = Mathf.SmoothDamp(landMoveAmount, curveSmooth.Evaluate(c.knobs[3]), ref landMoveAmountVelocity, lag);
		// dofVolumeFade = Mathf.SmoothDamp(dofVolumeFade, c.knobs[4], ref dofVolumeFadeVelocity, lag);
		hitchCockZoom = Mathf.SmoothDamp(hitchCockZoom, c.knobs[4], ref hitchCockVelocity, lag);
		ballScale = Mathf.SmoothDamp(ballScale, 0.2f + 30 * curveSmooth.Evaluate(c.knobs[5]), ref ballScaleVelocity, lag);
   		ballRippleScale = Mathf.SmoothDamp(ballRippleScale, 100 * curveSmooth.Evaluate(c.knobs[6]), ref ballRippleScaleVelocity, lag);
		bloomVolumeFade = Mathf.SmoothDamp(bloomVolumeFade, c.knobs[7], ref bloomVolumeFadeVelocity, lag);
		maskCover = Mathf.SmoothDamp(maskCover, 2.84f + 2.2f * curveSmooth.Evaluate(c.knobs[8]), ref maskCoverVelocity, lag);

		hitchCockDist = map(curveDolly.Evaluate(hitchCockZoom), 1.2f, .1f);

		// Sound react
        thisAudioSource.GetSpectrumData(Samples, 0, fftWindow);

		ShutterBall.SetActive(c.pads[1] ? true : false);
		CloudBall.SetActive(c.pads[2] ? true : false);
		dofVolume.weight = c.pads[3] ? 1 : 0;
		models.SetActive(c.pads[4] ? true : false);
		soundScale = Samples[sample] * (c.pads[5] ? 2 : 0);
		BG.color = new Color(255,255,255, c.pads[6] ? 1 : 0);
		CloudBG.SetActive(c.pads[7] ? true : false);
		PinkBG.SetActive(c.pads[8] ? true : false);



		// Set variables and account for sliders and soundScale
		if (ballHeightVelocity != 0) ball.material.SetFloat("_Height", ballHeight + soundScale);
		if (ballFresnelVelocity != 0) ball.material.SetFloat("_Fresnel", ballFresnel /* + soundScale */);
		if (ballSpeedVelocity != 0) ball.material.SetFloat("_Speed", ballSpeed /* + soundScale */);
		if (ballRippleScaleVelocity != 0) ball.material.SetFloat("_Scale", ballRippleScale /* + soundScale */);
		if (landMoveAmountVelocity != 0) foreach (Renderer r in ground) r.material.SetFloat("_Amount", landMoveAmount);
		if (landMoveSpeedVelocity != 0) foreach (Renderer r in ground) r.material.SetFloat("_Speed", landMoveSpeed);
		if (bgColorVelocity != 0) BG.color = new Color(255,255,255, bgColor);
		if (maskCoverVelocity != 0) foreach (SkinnedMeshRenderer r in landMasks) r.SetBlendShapeWeight(0, maskCover + soundScale);
		if (ballScaleVelocity != 0) ball.transform.localScale = new Vector3(ballScale, ballScale, ballScale);
		if (bloomVolumeFadeVelocity != 0) bloomVolume.weight = bloomVolumeFade + soundScale;
		if (dofVolumeFadeVelocity != 0) dofVolume.weight = dofVolumeFade + soundScale;

		if (hitchCockVelocity != 0) {
			cameraRoot.transform.localScale = new Vector3(hitchCockDist,hitchCockDist,hitchCockDist);
			foreach (Camera c in cameras) {
				c.fieldOfView = 15 + 100 * hitchCockZoom;
			}
		}
	}

	private void getMidi() {

		for (int i = 0; i < 9; i++) {
			if (MidiInput.GetKnob(i) >= 0) c.knobs[i] = MidiInput.GetKnob(i);
		}
		for (int i = 0; i < 9; i++) {
			if (MidiInput.GetPad(i) == 1) { c.pads[i] = true; } else { c.pads[i] = false; }
		}

	}	



	private void getRelativeMidi() {
		int k = MidiInput.LatestKnob;
		hiddenValues[k] += MidiInput.LatestValue;
		c.knobs[k] = 1f + (-0.5f * (1f + Mathf.Cos(Mathf.PI * hiddenValues[k])));
	}

	IEnumerator BallLerpValue (string val, float end) {
		float begin = ball.material.GetFloat(val);
		float t = 0;
		while (t < lag) {
			ball.material.SetFloat(val, Mathf.Lerp(begin, end, t/lag));
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
