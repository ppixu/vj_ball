using System;
using UnityEngine;

[Serializable]
public class Knob : Control
{
    [SerializeField]
    [Range(0f, 1f)]
    public float Value;
    [SerializeField]
    public FloatEvent OnValueChanged;

    public void SetValue(float newValue)
    {
        if (!Mathf.Approximately(Value, newValue))
        {
            OnValueChanged.Invoke(newValue);
        }
        Value = newValue;
    }
}
