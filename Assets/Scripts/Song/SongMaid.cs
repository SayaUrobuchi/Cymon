using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongMaid : MonoBehaviour
{
    public SongData Data;

    public AudioClip SongRef
    {
        get
        {
            return Data.SongRef;
        }
    }

    public string MainTitle
    {
        get
        {
            return Data.MainTitle;
        }
    }

    public string SubTitle
    {
        get
        {
            return Data.SubTitle;
        }
    }

    public float TempoLength
    {
        get
        {
            return Data.TempoLength;
        }
    }

    public List<SongClickPoint> ClickPoints
    {
        get
        {
            return Data.ClickPoints;
        }
    }

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void AddRecordToData(List<SongClickPoint> list)
    {
        Data.ClickPoints.AddRange(list);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
