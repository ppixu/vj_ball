using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MidiJack;

[RequireComponent(typeof(Controls))]
public class MidiController : MonoBehaviour
{
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
    AudioSource audioSource;

    private Controls controls;
    private float[] previousKnobValues;

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
    private bool soundScalePadValue = false;

    void Awake()
    {
        controls = GetComponent<Controls>();
        audioSource = GetComponent<AudioSource>();
        fftWindow = FFTWindow.Triangle;
    }

    void Update()
    {
        ReadMidi();
        // Store previous
        ReactToAudio();
        soundScale = Mathf.SmoothDamp(soundScale, Samples[sample] * (soundScalePadValue ? 2 : 0), ref soundScaleVelocity, .01f);

        ApplyInputValues();
    }

    private void ReadMidi()
    {
        if (!MidiInput.Receiving) return;
        GetRelativeMidi();
    }

    private void ReactToAudio()
    {
        audioSource.GetSpectrumData(Samples, 0, fftWindow);
    }

    #region control handlers

    public void SetMaskCover(float value)
    {
        maskCover = Mathf.SmoothDamp(maskCover, 2.84f + 2.2f * curveSmooth.Evaluate(value), ref maskCoverVelocity, .05f);
    }

    public void SetGroundAmount(float value)
    {
        for (int i = 0; i < ground.Length; i++)
        {
            ground[i].material.SetFloat("_Amount", curveSmooth.Evaluate(value));
        }
    }

    public void SetGroundSpeed(float value)
    {
        for (int i = 0; i < ground.Length; i++)
        {
            ground[i].material.SetFloat("_Speed", curveSmooth.Evaluate(value));
        }
    }

    public void SetBallSpeed(float value)
    {
        ballSpeed = Mathf.SmoothDamp(ballSpeed, curveSmooth.Evaluate(value), ref ballSpeedVelocity, .05f);
    }

    public void SetBallFresnel(float value)
    {
        ballFresnel = Mathf.SmoothDamp(ballFresnel, 100 * curveIn.Evaluate(value), ref ballFresnelVelocity, .05f);
    }

    public void SetBallRippleScale(float value)
    {
        ballRippleScale = Mathf.SmoothDamp(ballRippleScale, 100 * curveSmooth.Evaluate(value), ref ballRippleScaleVelocity, .05f);
    }

    private void SetBallHeight(float value)
    {
        ballHeight = Mathf.SmoothDamp(ballHeight, 3 - 6 * curveInverse.Evaluate(value), ref ballHeightVelocity, .05f);
    }

    public void SetBallScale(float value)
    {
        ballScale = Mathf.SmoothDamp(ballScale, 0.2f + 30 * curveSmooth.Evaluate(value), ref ballScaleVelocity, .05f);
    }

    private void SetBackgroundColor(float value)
    {
        var bgColor = BG.color;
        bgColor.a = curveSmooth.Evaluate(value);
        BG.color = bgColor;
    }

    public void SetSoundScalePadValue(bool value)
    {
        soundScalePadValue = value;
    }

    #endregion

    private void ApplyInputValues()
    {
        if (ballHeightVelocity != 0) ball.material.SetFloat("_Height", ballHeight + soundScale);
        if (ballFresnelVelocity != 0) ball.material.SetFloat("_Fresnel", ballFresnel /* + soundScale */);
        if (ballSpeedVelocity != 0) ball.material.SetFloat("_Speed", ballSpeed /* + soundScale */);
        if (ballRippleScaleVelocity != 0) ball.material.SetFloat("_Scale", ballRippleScale /* + soundScale */);
        foreach (SkinnedMeshRenderer r in landMasks) r.SetBlendShapeWeight(0, maskCover + soundScale);
        if (ballScaleVelocity != 0) ball.transform.localScale = new Vector3(ballScale, ballScale, ballScale);
    }

    private void GetMidi()
    {
        for (int i = 0; i < controls.Knobs.Count; i++)
        {
            if (MidiInput.GetKnob(i) >= 0) controls.SetKnob(i, MidiInput.GetKnob(i));
        }
        for (int i = 0; i < controls.Pads.Count; i++)
        {
            controls.SetPad(i, MidiInput.GetPad(i) == 1);
        }

    }

    private void GetRelativeMidi()
    {
        int k = MidiInput.LatestKnob;
        previousKnobValues[k] += MidiInput.LatestValue;
        controls.SetKnob(k, Mathf.PingPong(previousKnobValues[k], 1));
        for (int i = 0; i < controls.Pads.Count; i++)
        {
            controls.SetPad(i, MidiInput.GetPad(i) == 1);
        }
    }

    IEnumerator BallLerpValue(string val, float end)
    {
        float begin = ball.material.GetFloat(val);
        float t = 0;
        while (t < 0.05f)
        {
            ball.material.SetFloat(val, Mathf.Lerp(begin, end, t / 0.05f));
            t = t + Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    float Map(float x, float out_min, float out_max)
    {
        return (x - 0) * (out_max - out_min) / (1 - 0) + out_min;
    }
}
