using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongData : ScriptableObject
{
    public AudioClip SongRef;
    public string MainTitle = "";
    public string SubTitle = "";
    public float TempoLength = 1f;
    public List<SongClickPoint> ClickPoints = new List<SongClickPoint>();
}
