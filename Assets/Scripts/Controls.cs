using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controls : MonoBehaviour
{
	[Header("The values here are for display purposes only; modifying these won't send actual parameters into the model.")]

    [SerializeField]
    private List<Knob> knobs;
    [SerializeField]
    private List<Pad> pads;

    public List<Knob> Knobs { get { return knobs; } }
    public List<Pad> Pads { get { return pads; } }

    void Awake()
    {
        for (int i = 0; i < pads.Count; i++)
        {
            pads[i].SetIndex(i);
        }
        for (int i = 0; i < knobs.Count; i++)
        {
            knobs[i].SetIndex(i);
        }
    }

    public void SetKnob(int index, float value)
    {
        var knob = knobs[index];
        knob.SetValue(value);
    }

    public void SetPad(int index, bool value)
    {
        var pad = pads[index];
        pad.SetValue(value);
    }
}
