using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SongClickPoint
{
    public enum Type
    {
        Single = 0,
        Drag = 1,
        Sub = 2, 
    }

    public Type ClickType;
    public float TimePoint;
    public Vector2 Position;
    public float ExtraParameter;
}
