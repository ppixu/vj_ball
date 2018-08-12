using UnityEngine;

public class Control
{
    [HideInInspector]
    public int Index;

    public virtual void SetIndex(int index)
    {
        Index = index;
    }
}
