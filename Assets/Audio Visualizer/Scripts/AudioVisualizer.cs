﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioVisualizer : MonoBehaviour
{
    public GameObject[] Objects;
    public float Scale = 10f;
    public enum EnumType {X, Y, Z}
    public EnumType Type = EnumType.Y;

    private float[] Samples = new float[1024];

    FFTWindow fftWindow;
    AudioSource thisAudioSource;

    void Start()
    {
        thisAudioSource = GetComponent<AudioSource>();
        fftWindow = FFTWindow.Triangle;
    }


    void Update()
    {
        thisAudioSource.GetSpectrumData(Samples, 0, fftWindow);

        for (int i = 0; i < Objects.Length; i++)
        {
            var myScale = Samples[i] * Scale;

            Objects[0].transform.localScale = new Vector3(myScale, myScale, myScale);
        }
    }
}