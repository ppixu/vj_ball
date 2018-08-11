﻿using UnityEngine;
using System.Collections;

public class Racket : MonoBehaviour
{
    public int channel;
    public float moveWidth;

    void Update ()
    {
        transform.localPosition = Vector3.up * (moveWidth * (MidiInput.GetKnob(channel) - 0.5f));
    }
}