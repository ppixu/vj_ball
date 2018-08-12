using System;
using UnityEngine;

[Serializable]
public class Pad : Control
{
    [SerializeField]
    public bool Value;
    [SerializeField]
    public BoolEvent OnValueChanged;

    public void SetValue(bool newValue)
    {
        if (Value != newValue)
        {
            OnValueChanged.Invoke(newValue);
        }
        Value = newValue;
    }
}
