using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EditorControls : MonoBehaviour
{
    [Header("This script emulates MIDI control in editor")]
    [SerializeField]
    private Controls controls;

    [SerializeField]
    [Range(0f, 1f)]
    private float[] simulatedKnobs;
    private float[] cachedKnobValues;

    [SerializeField]
    private bool[] simulatedPads;
    private bool[] cachedPadValues;

#if !UNITY_EDITOR
    void Awake()
    {
        this.enabled = false;
    }
#endif

    void Update()
    {
        if (controls == null) return;
        if (cachedKnobValues == null) cachedKnobValues = simulatedKnobs.Clone() as float[];
        if (cachedPadValues == null) cachedPadValues = simulatedPads.Clone() as bool[];
        UpdateKnobs();
        UpdatePads();
    }

    private void UpdateKnobs()
    {
        for (int i = 0; i < simulatedKnobs.Length; i++)
        {
            if (!Mathf.Approximately(cachedKnobValues[i], simulatedKnobs[i]))
            {
                controls.SetKnob(i, simulatedKnobs[i]);
                cachedKnobValues[i] = simulatedKnobs[i];
            }
        }
    }

    private void UpdatePads()
    {
        for (int i = 0; i < simulatedPads.Length; i++)
        {
            if (cachedPadValues[i] != simulatedPads[i])
            {
                controls.SetPad(i, simulatedPads[i]);
                cachedPadValues[i] = simulatedPads[i];
            }
        }
    }
}
